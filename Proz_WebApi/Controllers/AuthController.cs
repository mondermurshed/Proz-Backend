using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Proz_WebApi.Models.Dto.Auth;
using Proz_WebApi.Services;
using Zxcvbn;

namespace Proz_WebApi.Controllers
{
    [ApiController]
    [Route("Clients")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
       
        private readonly AuthService _authservice;

        public AuthController(AuthService authservice)
        {
         
            _authservice = authservice;
        }
      
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult<string>> RegisterAUser([FromBody] UserRegisteration userregister)
        {


            var validation = PasswordChecker.ValidatePassword(
            userregister.Password,
            userregister.Email,
            userregister.Username
        );
            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    validation.Message,
                    validation.Score,
                    validation.Strength,
                    validation.CrackTime,
                    validation.Suggestions
                });
            }


       
            var result = await _authservice.RegisterAUser(userregister);
            if (result.Succeeded)
            {
                string PasswordInfo     = validation.Message;
                string PasswordStrength = validation.Strength;
                int PasswordRating = validation.Score;
                return Ok(new { 
                    Message = "Registration was successful",
                    PasswordInfo,
                    PasswordStrength,
                    PasswordRating          
                });
            }
            else
            {
                return BadRequest(new { Errors = result.Errors });
            }
        }
        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult<string>> LoginAUser([FromBody] UserLogin userlogin)
        {
            var result = await _authservice.LoginAUser(userlogin);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                });
            }
            else
            {
                return Unauthorized(new { Errors = result.Errors });
            }
        }

        
        [HttpPost("RefreshMyToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[AllowAnonymous] do NOT make this endpoint [Authorize] because the main goal of this endpoint (The refresh token) purpose is to renew an expired access token, so the endpoint must work even if the access token is invalid/expired.
        public async Task<ActionResult<string>> RefreshAToken([FromBody] AccessAndRefreshTokenPassing refreshToken)
        {
        var result=await _authservice.RefreshAToken(refreshToken);
        if (!result.Succeeded)
            {
                return Unauthorized(new { Errors = result.Errors });
            }
            string YourNewAccessToken = result.AccessToken;
            string YourNewRefreshToken = result.RefreshToken;
            return Ok(new
            {
                YourNewAccessToken,
                YourNewRefreshToken
            });
        }
    }
}
