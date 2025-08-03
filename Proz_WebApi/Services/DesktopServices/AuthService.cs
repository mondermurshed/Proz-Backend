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
using Proz_WebApi.Helpers_Services;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Amazon.SimpleEmailV2;
using Proz_WebApi.Models.DesktopModels;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using System.Web;
using Proz_WebApi.Models.DesktopModels.DTO.Auth;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Rewrite;
using System.Transactions;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Microsoft.AspNetCore.Http;

namespace Proz_WebApi.Services.DesktopServices
{
    public class AuthService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly JWTOptions _jwtoption;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AuthService> _loggerr;
        private readonly IEasyCachingProviderFactory _easyCachingFactory;
        private readonly EmailNormalizer _emailNormalizer;
        private readonly DomainVerifier _domainVerifier;
        private readonly SesEmailSender _EmailSender;
        private readonly VerificationCodeService _managingTempData;
        private readonly AesEncryptionService _aesEncryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager,
            JWTOptions jwtoption, ApplicationDbContext_Desktop dbcontext, ILogger<AuthService> loggerr, IEasyCachingProviderFactory easyCachingFactory,
            EmailNormalizer emailNormalizer, DomainVerifier domainVerifier, VerificationCodeService managingTempData
            , SesEmailSender EmailSender, AesEncryptionService aesEncryptionService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtoption = jwtoption;
            _dbcontext = dbcontext;
            _loggerr = loggerr;
            _easyCachingFactory = easyCachingFactory;
            _emailNormalizer = emailNormalizer;
            _domainVerifier = domainVerifier;
            _EmailSender = EmailSender;
            _managingTempData = managingTempData;
            _aesEncryptionService = aesEncryptionService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<FinalResult> RegisterAUserStageOne(UserRegisteration userregister)  //--------------------------------------------------
        {
            var finalresult = new FinalResult();
            string normalizedName = _emailNormalizer.NormalizeName(userregister.Username);
            string? normalizedEmail = _emailNormalizer.NormalizeEmail(userregister.Email);

            if (normalizedEmail == null)
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Add("Please enter a valid email that contains the '@' in it.");
                return finalresult;
            }

            if (await _managingTempData.IsInCooldownAsync(normalizedEmail))
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Add("Please wait a few seconds before trying to register again.");
                return finalresult;
            }
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);
            //if (user!=null)
            //{
            //    finalresult.Succeeded = false;
            //    finalresult.Errors.Add($"User with the email {normalizedEmail} is already stored in our databases!");
            //    return finalresult;
            //}
            bool domainexisting = await _domainVerifier.HasValidDomainAsync(normalizedEmail);
            if (!domainexisting)
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Add("The email's domain that you have entered is not able to receive emails");
                finalresult.Messages.Add("Please enter a valid email's domain in order to register");
                return finalresult;
            }
            string code;
            try
            {
                code = await _managingTempData.GenerateAndStoreCodeAsync(normalizedEmail);
            }
            catch (Exception ex)
            {
                finalresult.Succeeded = false;
                finalresult.Messages.Add("something went wront while saving user's data");
                finalresult.Errors.Add($"we got an error while saving the temp data of the user inside our server. {Environment.NewLine} Please try again in other time.");

                return finalresult;
            }
            string encryptedPassword = _aesEncryptionService.Encrypt(userregister.Password);
            var userTempData = new UserRegisterationTemp
            {
                Username = normalizedName,
                Email = normalizedEmail,
                Password = encryptedPassword,
                FullName= userregister.FullName,
                Date_Of_Birth = userregister.Date_Of_Birth,
                Age= userregister.Age,
                Gender=userregister.Gender,
                Nationality=userregister.Nationality,
                Living_On_Primary_Place=userregister.Living_On_Primary_Place
            };
            try
            {
                await _managingTempData.StoreUserRegistrationDataTemp(userTempData, normalizedEmail);
            }
            catch (Exception ex)
            {
                finalresult.Succeeded = false;
                finalresult.Messages.Add("something went wront while saving user's data");
                finalresult.Errors.Add($"we got an error while saving the temp data of the user inside our server. {Environment.NewLine} Please try again in other time.");
                return finalresult;
            }


            if (!await _EmailSender.SendVerificationCodeAsync(normalizedEmail, code))
            {
                finalresult.Succeeded = false;
                finalresult.Errors.Add("Something went wrong while trying to send the email. Please try again later");
                return finalresult;
            }
            await _managingTempData.StartCooldownAsync(normalizedEmail);
            finalresult.Succeeded = true;
            finalresult.Messages.Add($"Verification code has been sent to the email {normalizedEmail}, waiting the user to verify");
            return finalresult;
        }
        public async Task<FinalResult> RegisterAUserStageTwo(UserRegistrationFinal userregister) //--------------------------------------------------
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                var finalresult = new FinalResult();


                try
                {


                    string? normalizedEmail = _emailNormalizer.NormalizeEmail(userregister.Email);
                    if (normalizedEmail == null)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Please enter a valid email that contains the '@' in it.");
                        return finalresult;
                    }
                    bool isValid = await _managingTempData.ValidateCodeAsync(normalizedEmail, userregister.Code);
                    if (!isValid)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Invalid or expired verification code.");
                        return finalresult;
                    }
                    var cache = await _managingTempData.GetUserRegistrationDataTemp(normalizedEmail);
                    if (cache == null)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Registration data expired. Please try again..");
                        return finalresult;
                    }
                    string normalizeUserName = _emailNormalizer.NormalizeName(cache.Username); //we don't trust redis here so we are normalizing the username and not just taking the username from redis blindly (because an attacker that may use the server and may change some users's data, because if it did then strange data will enter the database for example username all upper case)
                    var user = new ExtendedIdentityUsersDesktop
                    {
                        UserName = normalizeUserName,
                        Email = normalizedEmail
                    };
                    string decryptedPassword = _aesEncryptionService.Decrypt(cache.Password);
                    var result = await _userManager.CreateAsync(user, decryptedPassword);  //UserManager and RoleManager automatically save changes when you call methods like CreateAsync or AddToRoleAsync. When you use _dbcontext directly(e.g., adding a refresh token), you must call SaveChangesAsync. This < must answer your question about why we don't put SaveChangesAsync in every time we interact with the database.

                    if (!result.Succeeded)
                    {
                        finalresult.Errors.AddRange(result.Errors.Select(e => e.Description));
                        finalresult.Succeeded = false;
                        return finalresult;
                    }
                    if (!await _roleManager.RoleExistsAsync(AppRoles_Desktop.User))
                    {

                        throw new RoleNotFoundException(AppRoles_Desktop.User, user.UserName); //the throw will send the message to the  catch (Exception ex) ex variable so you log the error and what happend exactly inside the try statement from the catch statement.
                    }
                    var roleResult = await _userManager.AddToRoleAsync(user, AppRoles_Desktop.User);
                    if (!roleResult.Succeeded)
                    {

                        finalresult.Succeeded = false;
                        finalresult.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                        return finalresult;
                    }
                  
                    await _dbcontext.PersonalInformationTable.AddAsync(new Personal_Information
                    {
                       FullName=cache.FullName,
                       Age=Convert.ToInt32( cache.Age),
                       DateOfBirth=cache.Date_Of_Birth,
                       Gender=cache.Gender,
                       Nationality=cache.Nationality,
                       LivingOnPrimaryPlace=cache.Living_On_Primary_Place,
                      
                        IdentityUser_FK =user.Id
                    });
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                    finalresult.Succeeded = true;
                    return finalresult;

                }

                catch (RoleNotFoundException ex) //this catch will only be executed if the RoleNotFoundException was thrown only but not the rest of other exception, so we have to create another global exception like   catch (Exception ex)
                {


                    _loggerr.LogError(ex, "User registration failed at {ErrorTime} because the role {RoleName} wasn't exisit to be assigned to the user {UserName}", ex.ErrorOccurredAt, ex.RoleName, ex.UserName);
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("Registration failed due to a system error.");
                    return finalresult; //we didn't send the exact error to the user because he doesn't need to know that we could assign a role to him because of the server's error so we just send to him "faild registeration" and we logged the exact error.

                }
                catch (Exception ex)
                {

                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("Un error occurred. Failed to register the user.");
                    return finalresult;
                }
            }
        }

        public async Task<FinalResult> ResendVerificationCodeAsync(ResendVerificationCodeDTO resendcode)  //--------------------------------------------------
        {
            var finalResult = new FinalResult();
            string? normalizedEmail = _emailNormalizer.NormalizeEmail(resendcode.Email);

            if (normalizedEmail == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Please enter a valid email.");
                return finalResult;
            }



            try
            {
                if (await _managingTempData.IsInCooldownAsync(normalizedEmail))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Please wait few seconds from the moment you sent the previous code!");
                    return finalResult;
                }
                var existingData = await _managingTempData.GetUserRegistrationDataTemp(normalizedEmail);
                if (existingData == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("No pending registration found for this email.");
                    return finalResult;
                }
                string newCode = await _managingTempData.GenerateAndStoreCodeAsync(normalizedEmail);
                await _managingTempData.StoreUserRegistrationDataTemp(existingData, normalizedEmail);
                if (!await _EmailSender.SendVerificationCodeAsync(normalizedEmail, newCode))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Something went wrong while trying to send the email. Please try again later");
                    return finalResult;
                }
                await _managingTempData.StartCooldownAsync(normalizedEmail);
                finalResult.Succeeded = true;
                finalResult.Messages.Add("Verification code has been resent to your email.");

            }
            catch (Exception ex)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("An error occurred. Please try again later.");
            }

            return finalResult;
        }
        public async Task<FinalResult> ForgotMyPassword(ForgotPasswordDTO forgetpassword)  //--------------------------------------------------
        {
            var finalResult = new FinalResult();
            string? normalizedEmail = _emailNormalizer.NormalizeEmail(forgetpassword.Email);

            if (normalizedEmail == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Please enter a valid email");
                return finalResult;
            }

            var user = await _userManager.FindByEmailAsync(normalizedEmail);
            if (user == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("No email like this was found. Please enter a valid/correct email");
                return finalResult;
            }
            if (await _managingTempData.IsInCooldownAsync(normalizedEmail))
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Please wait few seconds from the moment you have sent the previous send operation");
                return finalResult;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user); //identity will not store this token inside the database but this token was generated on-the-fly using a combination of user data, a secret key, and a purpose ("ResetPassword"). When the user sends back the token to reset the password, the Identity system re-generates what it expects and compares it to the one you sent.

            var resetLink = $"https://reset.prozsupport.xyz/ResetPassword/ResetPassword.html?token={HttpUtility.UrlEncode(token)}&email={normalizedEmail}"; //Why Do We Need UrlEncode()? because the token will contain special characters, like: / or + or =  So if you put the token directly into a link like this: https://reset.prozsupport.xyz/ResetPassword/ResetPassword.html?token=CfDJ8EozwIS6qN/ozLYVRHVc8FvKX7fvh9DxI+oBHRaM5VmPOonZb==&email=user@email.com The Solution: HttpUtility.UrlEncode(token) This function transforms the token into a safe version for URLs. Original token: CfDJ8EozwIS6qN/ozLYVRHVc8FvKX7fvh9DxI+oBHRaM5VmPOonZb==  After encoding: CfDJ8EozwIS6qN%2FozLYVRHVc8FvKX7fvh9DxI%2BoBHRaM5VmPOonZb%3D%3D

            if (!await _EmailSender.SendResetLinkAsync(normalizedEmail, resetLink))
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Something went wrong while trying to send the email. Please try again later");
                return finalResult;
            }
            //await _managingTempData.StartCooldownAsync(normalizedEmail); //after you complete from testing, uncomment this line <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            finalResult.Succeeded = true;
            finalResult.Messages.Add("An email was sent successfully to your address. Please check your inbox.");
            return finalResult;
        }


        public async Task<FinalResultWithPasswordCheckingInfo> ResetPassword(ResetPasswordDto resetpassword) //--------------------------------------------------
        {
            var finalResult = new FinalResultWithPasswordCheckingInfo();
            string? normalizedEmail = _emailNormalizer.NormalizeEmail(resetpassword.Email);

            if (normalizedEmail == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Your end didn't pass the email correctly to complete the process. Please make sure that you clicked on a correct/new token");
                return finalResult;
            }
            var user = await _userManager.FindByEmailAsync(resetpassword.Email);
            if (user == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("No user with this email was found. Please try to request for a new reset-password process");
                return finalResult;
            }
            var validation = PasswordChecker.ValidatePassword(
            resetpassword.NewPassword,
            user.Email,
            user.UserName

            );
            if (!validation.IsValid)
            {
                finalResult.Succeeded = false;
                finalResult.NewPasswordCause = true;
                finalResult.Errors.Add(validation.Message);
                finalResult.Score = validation.Score;
                finalResult.CrackTime = validation.CrackTime;
                finalResult.Strength = validation.Strength;
                finalResult.Suggestions.AddRange(validation.Suggestions);
                return finalResult;
            }
            var resetResult = await _userManager.ResetPasswordAsync(user, resetpassword.Token, resetpassword.NewPassword);
            if (!resetResult.Succeeded)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.AddRange(resetResult.Errors.Select(e => e.Description));
                return finalResult;
            }

            finalResult.Succeeded = true;
            finalResult.Messages.Add("Your password has been reset successfully.");
            return finalResult;
        }

        public async Task<FinalResult> RegisterAUser(UserRegisteration userregister) //--------------------------------------------------
        {
            await using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();



            try
            {
                string normalizedName = _emailNormalizer.NormalizeName(userregister.Username);
                string? normalizedEmail = _emailNormalizer.NormalizeEmail(userregister.Email);
                if (normalizedEmail == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Please enter a valid email that contains the '@' in it.");
                    return finalresult;
                }

                bool domainexisting = await _domainVerifier.HasValidDomainAsync(normalizedEmail);
                if (!domainexisting)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The email's domain that you have entered is not able to receive mails");
                    finalresult.Messages.Add("Please enter a valid email's domain in order to register to the service");
                    return finalresult;
                }


                var user = new ExtendedIdentityUsersDesktop
                {
                    UserName = userregister.Username,
                    Email = userregister.Email



                };

                var result = await _userManager.CreateAsync(user, userregister.Password);  //UserManager and RoleManager automatically save changes when you call methods like CreateAsync or AddToRoleAsync. When you use _dbcontext directly(e.g., adding a refresh token), you must call SaveChangesAsync. This < must answer your question about why we don't put SaveChangesAsync in every time we interact with the database.


                if (!result.Succeeded)
                {
                    finalresult.Errors.Clear();
                    finalresult.Errors.AddRange(result.Errors.Select(e => e.Description));
                    finalresult.Succeeded = false;
                    return finalresult;
                }
                if (!await _roleManager.RoleExistsAsync(AppRoles_Desktop.Admin))
                {

                    throw new RoleNotFoundException(AppRoles_Desktop.Admin, user.UserName); //the throw will send the message to the  catch (Exception ex) ex variable so you log the error and what happend exactly inside the try statement from the catch statement.
                }

                var roleResult = await _userManager.AddToRoleAsync(user, AppRoles_Desktop.Admin);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                    return finalresult;
                }
                await transaction.CommitAsync();
                finalresult.Succeeded = true;
                return finalresult;
            }
            catch (RoleNotFoundException ex) //this catch will only be executed if the RoleNotFoundException was thrown only but not the rest of other exception, so we have to create another global exception like   catch (Exception ex)
            {

                await transaction.RollbackAsync();
                _loggerr.LogError(ex, "User registration failed at {ErrorTime} because the role {RoleName} wasn't exisit to be assigned to the user {UserName}", ex.ErrorOccurredAt, ex.RoleName, ex.UserName);
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
        public async Task<FinalResult> LoginAUser(UserLogin userlogin) //---------------------------------------------------
        {
            var finalresult = new FinalResult();
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    ExtendedIdentityUsersDesktop? user;

                    if (userlogin.Username.Contains("@"))
                    {
                        user = await _userManager.FindByEmailAsync(userlogin.Username);
                    }
                    else
                    {
                        user = await _userManager.FindByNameAsync(userlogin.Username);
                    }

                    if (user == null)
                    {
                        finalresult.Succeeded = false;

                        finalresult.Errors.Add("User is not located inside the database");
                        return finalresult;
                    }

                    if (!await _userManager.CheckPasswordAsync(user, userlogin.Password)) //since our password is stored into the database as a hashed password (for security puposes so no one who has access to the database to know the users passwords) we can only compare the user's provided password to the user's hashed password that is stored in the database only by CheckPasswordAsync method.
                    {
                        finalresult.Succeeded = false;

                        finalresult.Errors.Add("An account was found but the password is not correct");
                        return finalresult;
                    }

                    var accessToken = await GenerateJwtToken(user); //<added this one the first argument.
                    var (refreshtoken, hash) = GenerateRefreshToken(); //Because GenerateRefreshToken method returns two parameters then here we create two variables to take the two values that they are returned by this method

                    string deviceTokenHash = HashDeviceToken(userlogin.DeviceToken);

                    // Find old unexpired token for same user + same device
                    var existing = await _dbcontext.RefreshTokensTable.FirstOrDefaultAsync(rt =>
                        rt.UserFK == user.Id &&
                        rt.DeviceTokenHash == deviceTokenHash &&
                        rt.IsRevoked == false &&
                        rt.ExpiresAt > DateTime.UtcNow);

                    if (existing != null)
                    {
                        existing.IsRevoked = true; 
                    }



                    await _dbcontext.RefreshTokensTable.AddAsync(new RefreshTokenDesktop
                    {
                        UserFK = user.Id,
                        TokenHash = hash,
                        DeviceTokenHash = deviceTokenHash, 
                        DeviceName = userlogin.DeviceName, 
                        ExpiresAt = DateTime.UtcNow.AddDays(3)
                    });
                    await _dbcontext.LoginHistoryTable.AddAsync(new LoginHistory
                    {
                        ExtendedIdentityUsersDesktop_FK = user.Id,
                        LoggedAt = DateTime.UtcNow,
                        DeviceTokenhashed = deviceTokenHash,
                        DeviceName= userlogin.DeviceName,

                    });
                    user.LastOnline = DateTime.UtcNow; //update the user's last online time to the current time
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                    finalresult.AuthSuccess(accessToken, refreshtoken);
                }

                catch (Exception a)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Something went wrong, please try again");
                    return finalresult;
                }
            }
            return finalresult;
        }


        private async Task<string> GenerateJwtToken(ExtendedIdentityUsersDesktop user) //this method is to generate the access token along with all the settings needed for the user and its properties like id, name etc...
        {
            var tokenhandler = new JwtSecurityTokenHandler(); //Creates a tool that can create/read JWT tokens
            var key = Encoding.UTF8.GetBytes(_jwtoption.SigningKey); //your key that you have types in a bytes version.
            var roles = await _userManager.GetRolesAsync(user); //this will get all the roles of this user that are stored inside the database. Noticed that we have putted it as await because we retrieve data from the database.

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //The Sub identity something unique about the user (the owner of the token) like his ID or Username or Email. in most time we put the ID in it.
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //this claim gave you some benefits and features (read about it)
            new Claim(JwtRegisteredClaimNames.Aud, _jwtoption.Audience), //This to set the Aud of the token (it's meant to be used by who?) we create a claim as well for this only if we want to know the Audience of the token in our C# for any kind of purposes.
            new Claim(JwtRegisteredClaimNames.Iss, _jwtoption.Issuer), //It's to know this token mean to arrieve where exactly ? the purpose of defining it as a claim is the same purpose of the Audience claim.
            new Claim("TheCallerUserName", user.UserName), //This is a custom Claim (we use custom Claim only if you want to get the information that it holds in your logic (C# logic). Custom claim are means for the programmer only and not for the JWT libary.
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("TheCallerID", user.Id.ToString()),//This is the custom claim version of new Claim(JwtRegisteredClaimNames.Sub, userid). We can use both no problem
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



        private (string Token, string Hash) GenerateRefreshToken() //this method is to generate the refresh token with its hash value
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

        private string HashDeviceToken(string deviceToken) // this method is to take the device token and convert the hash value of that device token
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(deviceToken));
            return Convert.ToBase64String(bytes);
        }




        public async Task<FinalResult> RefreshAToken(DeviceTokenAndRefreshTokenPassing request) //this is only happen when the user's computer wants new access + refresh tokens without log in 
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                var finalresult = new FinalResult();
                // 1. Hash the incoming refresh token with SHA256
                using var sha256 = SHA256.Create(); //go read the lines (the last lines maybe ?? idk go check them) of the GenerateRefreshToken method to understand these.
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
                var tokenHash = Convert.ToBase64String(hashBytes);
             
                // 2. Find matching token
                var storedToken = await _dbcontext.RefreshTokensTable //This object will hold the whole refreshtokentable row (if it found or matched our requiremnts)
              .Include(rt => rt.UserNA) // Load user data (like if we found our desire row that match our 3 conditions (look the second code to understand what i mean) then we load the user that own this refreshtoken row.
              .FirstOrDefaultAsync(rt =>  //this will Searches for the first row in the database for a valid token matching 3 conditions:
                  rt.TokenHash == tokenHash && // 1- Match the user hash
                  rt.DeviceTokenHash == HashDeviceToken(request.DeviceToken) &&
                  rt.ExpiresAt > DateTime.UtcNow && // 2- Not expired yet
                  !rt.IsRevoked); // 3- still wasn't revoked yet



                if (storedToken == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("Unable to send a valid access token to the user, please re signing again");
                    return finalresult;
                }


                // 3. Revoke old token
                storedToken.IsRevoked = true; //here we are following something called "token rotation" which is a concept that mean if the user wants a new access token and he provide both his access and refresh then we don't just create an access token and make him keep his old refresh one but we are also changing his refresh token as well. At the end we give him a brand new access and refresh tokens (just to clarify all our talk are valid only if the user has a valid and unexpired and unrevoked refresh token already stored in out database) so in simple words old tokens become unusable.

                // 4. Generate new tokens
                var (newRefreshToken, newHash) = GenerateRefreshToken();
                var newAccessToken = await GenerateJwtToken(storedToken.UserNA);




                // 5. Store new token
                await _dbcontext.RefreshTokensTable.AddAsync(new RefreshTokenDesktop
                {
                    UserFK = storedToken.UserNA.Id,
                    TokenHash = newHash,
                    DeviceTokenHash = storedToken.DeviceTokenHash, // same device
                    DeviceName = storedToken.DeviceName, // preserve device info
                    ExpiresAt = DateTime.UtcNow.AddDays(3)
                });

                await _dbcontext.SaveChangesAsync();
                finalresult.Succeeded = true;
                finalresult.AuthSuccess(newAccessToken, newRefreshToken);
                scope.Complete();
                return finalresult;
            }
        }
     

        public async Task<FinalResult> ChangeUsernameAsync(string userId, ChangeUsernameDTO dto)
        {
            var finalResult = new FinalResult();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("User not found.");
                return finalResult;
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Current password is incorrect.");
                return finalResult;
            }

            string newUsername = _emailNormalizer.NormalizeName(dto.NewUsername);
            var existingUser = await _userManager.FindByNameAsync(newUsername);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Username already taken. Try a different one.");
                return finalResult;
            }

            user.UserName = newUsername;
            user.NormalizedUserName = newUsername.ToUpper(); // Important for ASP.NET Identity lookups

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.AddRange(result.Errors.Select(e => e.Description));
                return finalResult;
            }

            finalResult.Succeeded = true;
            finalResult.Messages.Add("Username changed successfully.");
            return finalResult;
        }

        public async Task<FinalResult> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            var finalResult = new FinalResult();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("User not found.");
                return finalResult;
            }

            var validation = (PasswordChecker.ValidatePassword(dto.NewPassword, user.Email, user.UserName));
            if (!validation.IsValid)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("New password needs improvement!.");
                return finalResult;
            }
        




            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.AddRange(result.Errors.Select(e => e.Description));
                return finalResult;
            }

            finalResult.Succeeded = true;
            finalResult.Messages.Add("Password changed successfully.");
            return finalResult;
        }

        public async Task<FinalResult> LogoutUser(LogoutRequest request)
        {
            var result = new FinalResult();

            try
            {
                using var sha256 = SHA256.Create();

                var refreshTokenHash = Convert.ToBase64String(
                    sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken)));

                var deviceTokenHash = HashDeviceToken(request.DeviceToken);

                var storedToken = await _dbcontext.RefreshTokensTable
                    .FirstOrDefaultAsync(rt =>
                        rt.TokenHash == refreshTokenHash &&
                        rt.DeviceTokenHash == deviceTokenHash &&
                        !rt.IsRevoked);

                if (storedToken is null)
                {
                    result.Succeeded = false;
                    result.Errors.Add("Refresh token not found or already revoked.");
                    return result;
                }

                storedToken.IsRevoked = true;
                await _dbcontext.SaveChangesAsync();

                result.Succeeded = true;
                return result;
            }
            catch
            {
                result.Succeeded = false;
                result.Errors.Add("An error occurred while logging out.");
                return result;
            }
        }











    }

}
