using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proz_WebApi.Models;
using Proz_WebApi.Models.Dto;
using Serilog;

namespace Proz_WebApi.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JWTOptions _jwtoption;

        public AuthService(UserManager<IdentityUser> userManager, JWTOptions jwtoption)
        {
            _userManager = userManager;
            _jwtoption = jwtoption;
        }
        public async Task<bool> RegisterAUser(UserRegisteration userlogin)
        {
            var identityuser = new IdentityUser
            {
                UserName = userlogin.Username,
                Email = userlogin.Email

            };
            var result = await _userManager.CreateAsync(identityuser, userlogin.Password);
            return result.Succeeded;
        }
        public async Task<bool> LoginAUser(UserLogin userlogin)
        {
            var identityuser = await _userManager.FindByNameAsync(userlogin.Username);
            if (identityuser == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(identityuser, userlogin.Password);
        }
        public async Task<string> GetJwtToken(UserLogin userlogin)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtoption.Issuer,
                Audience = _jwtoption.Audience,
                SigningCredentials =  new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtoption.SigningKey)), SecurityAlgorithms.HmacSha256),
                Expires = DateTime.UtcNow.AddHours(1),
                Subject = new ClaimsIdentity(new Claim[]
                {
                new (ClaimTypes.NameIdentifier,userlogin.Username),
       
                })
            };
            var securitytoken =  tokenhandler.CreateToken(tokendescriptor);
            var accesstoken =    tokenhandler.WriteToken(securitytoken);
            return accesstoken;
        }
        
    
         
    }
}
