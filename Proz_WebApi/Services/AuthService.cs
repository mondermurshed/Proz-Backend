using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Proz_WebApi.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Azure.Core;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.X86;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Controllers;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Models.Dto.Auth;
using Proz_WebApi.Helpers_Services;

namespace Proz_WebApi.Services
{
    public class AuthService
    {
        private readonly UserManager<ExtendedIdentityUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTOptions _jwtoption;
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<AuthService> _loggerr;


        public AuthService(UserManager<ExtendedIdentityUsers> userManager, RoleManager<IdentityRole> roleManager, JWTOptions jwtoption, ApplicationDbContext dbcontext, ILogger<AuthService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtoption = jwtoption;
            _dbcontext = dbcontext;
            _loggerr = loggerr;
        }
        public async Task<FinalResult> RegisterAUser(UserRegisteration userregister)
        {
            await using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();
            EmailNormalizer normalizer = new EmailNormalizer();
          
            try
            {
                string normalizedName = normalizer.NormalizeName(userregister.Username);
                string normalizedEmail=normalizer.NormalizeEmail(userregister.Email);
                var part = userregister.Email.Split('@');
                bool existing =await EmailDomainValidator.IsValidEmailDomainAsync(part[1]);
                if (existing == false)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The email domain that you have entered is not existed or it's not able to receive mails");
                    finalresult.Messages.Add("Please enter a valid email's domain in order to register");
                    return finalresult;
                }
            var user = new ExtendedIdentityUsers
            {
                UserName = userregister.Username,
                Email = userregister.Email
                
               
                
            };
     
            var result = await _userManager.CreateAsync(user, userregister.Password);  //UserManager and RoleManager automatically save changes when you call methods like CreateAsync or AddToRoleAsync. When you use _dbcontext directly(e.g., adding a refresh token), you must call SaveChangesAsync. This < must answer your question about why we don't put SaveChangesAsync in every time we interact with the database.


            if (!result.Succeeded)
            {
                    finalresult.Errors.Clear();
                   finalresult.Errors.AddRange(result.Errors.Select(e=>e.Description));
                    finalresult.Succeeded = false;
                    return finalresult;
            }
                if (!await _roleManager.RoleExistsAsync(AppRoles.User))
                {
                    
                    throw new RoleNotFoundException(AppRoles.Employee,user.UserName); //the throw will send the message to the  catch (Exception ex) ex variable so you log the error and what happend exactly inside the try statement from the catch statement.
                }

                var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);
            if (!roleResult.Succeeded)
            {
                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.AddRange(roleResult.Errors.Select(e=>e.Description));
                    return finalresult;
            }
                await transaction.CommitAsync();
                finalresult.Succeeded = true;
                return finalresult;
            }
            catch (RoleNotFoundException ex) //this catch will only be executed if the RoleNotFoundException was thrown only but not the rest of other exception, so we have to create another global exception like   catch (Exception ex)
            {
               
                await transaction.RollbackAsync();
                _loggerr.LogError(ex, "User registration failed at {ErrorTime} because the role {RoleName} wasn't exisit to be assigned to the user {UserName}", ex.ErrorOccurredAt,ex.RoleName,ex.UserName);
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add("Registration failed due to a system error.");
                return finalresult; //we didn't send the exact error to the user because he doesn't need to know that we could assign a role to him because of the server's error so we just send to him "faild registeration" and we logged the exact error.
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add("Un error occurred. Failed to register the user.");
                return finalresult;
            }
            
        }
        public async Task<FinalResult> LoginAUser(UserLogin userlogin)
        {
            var finalresult = new FinalResult();
            var user = await _userManager.FindByNameAsync(userlogin.Username); //here is to get the user object with all the row's information
            if (user == null)
            {
                finalresult.Succeeded=false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add("User is not located inside the database");
                return finalresult;
            }

            if (!await _userManager.CheckPasswordAsync(user, userlogin.Password)) //since our password is stored into the database as a hashed password (for security puposes so no one who has access to the database to know the users passwords) we can only compare the user's provided password to the user's hashed password that is stored in the database only by CheckPasswordAsync method.
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add("Password is not correct");
                return finalresult;
            }
        
            var accessToken = await GenerateJwtToken(user); //<added this one the first argument.
            var (refreshtoken, hash) =  GenerateRefreshToken(); //Because GenerateRefreshToken method returns two parameters then here we create two variables to take the two values that they are returned by this method
            await _dbcontext.RefreshTokensTable.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hash,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _dbcontext.SaveChangesAsync();
            finalresult.AuthSuccess(accessToken, refreshtoken);
            return finalresult;
        }

    
        private async Task< string> GenerateJwtToken(ExtendedIdentityUsers user)
        {
            var tokenhandler = new JwtSecurityTokenHandler(); //Creates a tool that can create/read JWT tokens
            var key = Encoding.UTF8.GetBytes(_jwtoption.SigningKey); //your key that you have types in a bytes version.
            var roles = await _userManager.GetRolesAsync(user); //this will get all the roles of this user that are stored inside the database. Noticed that we have putted it as await because we retrieve data from the database.

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), //The Sub identity something unique about the user (the owner of the token) like his ID or Username or Email. in most time we put the ID in it.
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //this claim gave you some benefits and features (read about it)
            new Claim(JwtRegisteredClaimNames.Aud, _jwtoption.Audience), //This to set the Aud of the token (it's meant to be used by who?) we create a claim as well for this only if we want to know the Audience of the token in our C# for any kind of purposes.
            new Claim(JwtRegisteredClaimNames.Iss, _jwtoption.Issuer), //It's to know this token mean to arrieve where exactly ? the purpose of defining it as a claim is the same purpose of the Audience claim.
            new Claim("TheCallerUserName", user.UserName), //This is a custom Claim (we use custom Claim only if you want to get the information that it holds in your logic (C# logic). Custom claim are means for the programmer only and not for the JWT libary.
            new Claim(JwtRegisteredClaimNames.Email, user.UserName),
            new Claim("TheCallerID", user.Id),//This is the custom claim version of new Claim(JwtRegisteredClaimNames.Sub, userid). We can use both no problem
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            // Add roles as claims
            foreach (var role in roles)
            {
             
                claims.Add(new Claim(ClaimTypes.Role, role)); 
            }
            if (user.EmailConfirmed)
            {
                claims.Add(new Claim("EmailVerified", "true")); //this is a cursom claim. the EmailVerified is the name of the claim and true is the value of it.
            }
            var tokendescriptor = new SecurityTokenDescriptor //// All the token settings and information go here
            {
                Issuer = _jwtoption.Issuer,
                Audience = _jwtoption.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtoption.LifetimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256), //this line is meant to take our key (the string of letters and digits that we have putted in bytes) and then try to hash it with the algorithm that we have chosen. So in simple terms it trys to cook something that it is hard for the hackers to guess it by just using our key and our algorithm.
                Subject = new ClaimsIdentity(claims) //in the subject section we define User Information which are claims. We use clamis so we can later know all the inforamtion of the user when he call an endpoint! JWT provide built in properties for this purpose. This is updated! i was putting all the claims inside the Subject object but now i create a new list of claims and then assign our subject to this claims object.
            };
            var token = tokenhandler.CreateToken(tokendescriptor); //Create the token using all the settings
            return tokenhandler.WriteToken(token); //Convert it to a string format that can be sent to clients and then return it
        }
        private (string Token, string Hash) GenerateRefreshToken()
        {
            var randombytes = new byte[64];
            using var randomnumbergenerator = RandomNumberGenerator.Create();
            randomnumbergenerator.GetBytes(randombytes);
            var token = Convert.ToBase64String(randombytes);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token)); //here it will take our string of letters and convert it to the hashed version of it but as bytes
            var hash = Convert.ToBase64String(hashBytes); //here we will take the bytes version of our hashed code and convert it to the string code.
            // first of all i was using the BCrypt.Net-Next library techniques but turn out that the library is not for tokens but for password because if you give a random string to be hashed by this library then it will generate the hash but if you provide later the same random string it will create a different hash code then the previous! (but of course the library itself know how to decode this code but still bad for tokens) this is why we used sha256 because you give it a string it will give you the hash code of it and if you give it the same string it will give the same exact hash code again. So the sha256 follows Deterministic way which is Same input ➔ same output every time but the Non-deterministic is Same input ➔ different output each time.
            return (token, hash);
        }

        public async Task<FinalResult> RefreshAToken(AccessAndRefreshTokenPassing request)
        {
            // 1. Hash the incoming refresh token with SHA256
            using var sha256 = SHA256.Create(); //go read the lines (the last lines maybe ?? idk go check them) of the GenerateRefreshToken method to understand these.
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var tokenHash = Convert.ToBase64String(hashBytes);
            var finalresult = new FinalResult();
            // 2. Find matching token
            var storedToken = await _dbcontext.RefreshTokensTable //This object will hold the whole refreshtokentable row (if it found or matched our requiremnts)
          .Include(rt => rt.User) // Load user data (like if we found our desire row that match our 3 conditions (look the second code to understand what i mean) then we load the user that own this refreshtoken row.
          .FirstOrDefaultAsync(rt =>  //this will Searches for the first row in the database for a valid token matching 3 conditions:
              rt.TokenHash == tokenHash && // 1- Match the user hash
              rt.ExpiresAt > DateTime.UtcNow && // 2- Not expired yet
              !rt.IsRevoked); // 3- still wasn't revoked yet



            if (storedToken == null)
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add("Unable to send a valid access token to the user");
                return finalresult;
            }
               
            
            // 3. Revoke old token
            storedToken.IsRevoked = true; //here we are following something called "token rotation" which is a concept that mean if the user wants a new access token and he provide both his access and refresh then we don't just create an access token and make him keep his old refresh one but we are also changing his refresh token as well. At the end we give him a brand new access and refresh tokens (just to clarify all our talk are valid only if the user has a valid and unexpired and unrevoked refresh token already stored in out database) so in simple words old tokens become unusable.
        
            // 4. Generate new tokens
            var (newRefreshToken, newHash) = GenerateRefreshToken();
            var newAccessToken = await GenerateJwtToken(storedToken.User);




            // 5. Store new token
            await _dbcontext.RefreshTokensTable.AddAsync(new RefreshToken
            {
                UserId = storedToken.User.Id,
                TokenHash = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _dbcontext.SaveChangesAsync();
            finalresult.AuthSuccess(newAccessToken, newRefreshToken);
            return finalresult;

        }








    }
  
}
