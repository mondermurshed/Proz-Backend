using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Amazon.SimpleEmailV2.Model;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Proz_WebApi.Models.DesktopModels;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using Proz_WebApi.Models.DesktopModels.DTO.Auth;
using Proz_WebApi.Services.DesktopServices;
using Zxcvbn;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [ApiController]
    [Route("Auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authservice;

        public AuthController(AuthService authservice)
        {

            _authservice = authservice;
           
        }

        [Route("RegisterStageOne")]
        [HttpPost]
        public async Task<ActionResult<string>> RegisterAUserStageOne([FromBody] UserRegisteration userregister)
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



            var result = await _authservice.RegisterAUserStageOne(userregister);
            if (result.Succeeded)
            {
                string PasswordInfo = validation.Message;
                string PasswordStrength = validation.Strength;
                int PasswordRating = validation.Score;
                return Ok(new
                {
                    Message = $"A verification code has been sent successfully to your email service, please re type the code here. {Environment.NewLine} The code validity period is one minute",
                    PasswordInfo,
                    PasswordStrength,
                    PasswordRating
                });
            }
            else
            {
                return BadRequest(new
                {
                   Message = result.Messages,
                   Error = result.Errors

                });
            }
        }
        [Route("RegisterStageTwo")]
        [HttpPost]
 
        public async Task<ActionResult<string>> RegisterAUserStageTwo([FromBody] UserRegistrationFinal userregister)
        {
            var result = await _authservice.RegisterAUserStageTwo(userregister);
            if (result.Succeeded)
            {

                return Ok(new
                {
                    Message = "Your account has been successfully created!"
                });
            }
            else
            {
                return BadRequest(new
                {
                   Message = result.Messages,
                   Error = result.Errors


                });
            }
        }
        [Route("ResendCode")]
        [HttpPost]

        public async Task<ActionResult<string>> ResendVerificationCodeAsync([FromBody] ResendVerificationCodeDTO resendcode)
        {
            var result = await _authservice.ResendVerificationCodeAsync(resendcode);
            if (result.Succeeded)
            {

                return Ok(new
                {
                    Message = result.Messages
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = result.Messages,
                    Error = result.Errors


                });
            }
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
                string PasswordInfo = validation.Message;
                string PasswordStrength = validation.Strength;
                int PasswordRating = validation.Score;
                return Ok(new
                {
                    Message = $"A verification code has been sent successfully to your email service, please re type the code here. {Environment.NewLine} The code validity period is one minute",
                    PasswordInfo,
                    PasswordStrength,
                    PasswordRating
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = result.Messages,
                    Error = result.Errors

                });
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
                    result.AccessToken,
                    result.RefreshToken
                });
            }
            else
            {
                return Unauthorized(new { result.Errors });
            }
        }
        [Route("ForgotMyPassword")]
        [HttpPost]

        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO foregetpassword)
        {
            var result = await _authservice.ForgotMyPassword(foregetpassword);
            if (result.Succeeded)
            {
                return Ok(new
                {
                   Message = result.Messages
                });
            }
            else
            {
                if (result.Messages==null)
                return BadRequest(new
                {
               Error = result.Errors
                });

             return BadRequest(new
             {
           Message =  result.Messages,
           Error = result.Errors
                });
            }
              

        }
       
        [HttpPost("ResetPassword")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetpassword)
        {
           
            var result = await _authservice.ResetPassword(resetpassword);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = result.Messages
                });
            }
            else if (!result.Succeeded && result.NewPasswordCause == true)
            {
                return BadRequest(new
                {
                    Error = result.Errors,
                    PasswordScore = result.Score,
                    CrackTime = result.CrackTime,
                    Suggestions = result.Suggestions

                });
            }
            else
            {
                if (result.Messages == null)
                    return BadRequest(new
                    {
                        Error = result.Errors
                    });

                return BadRequest(new
                {
                    Message = result.Messages,
                    Error = result.Errors
                });
            }


        }


        [HttpPost("RefreshMyToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[AllowAnonymous] do NOT make this endpoint [Authorize] because the main goal of this endpoint (The refresh token) purpose is to renew an expired access token, so the endpoint must work even if the access token is invalid/expired.
        public async Task<ActionResult<string>> RefreshAToken([FromBody] AccessAndRefreshTokenPassing refreshToken)
        {
            var result = await _authservice.RefreshAToken(refreshToken);
            if (!result.Succeeded)
            {
                return Unauthorized(new { result.Errors });
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
