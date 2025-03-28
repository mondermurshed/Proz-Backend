using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Proz_WebApi.Models.Dto;
using Proz_WebApi.Services;

namespace Proz_WebApi.Controllers
{
    [ApiController]
    [Route("Clients")]

    public class AuthController : ControllerBase
    {
       
        private readonly AuthService _authservice;

        public AuthController(AuthService authservice)
        {
         
            _authservice = authservice;
        }
        //[HttpPost]
        //[Route("Register1")]
        //public ActionResult<string> RegisterAUser1([FromBody] UserRegisteration login)
        //{
        //    var tokenhandler = new JwtSecurityTokenHandler();
        //    var tokendescriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = _jwtoption.Issuer,
        //        Audience = _jwtoption.Audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtoption.SigningKey)), SecurityAlgorithms.HmacSha256),
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //        new (ClaimTypes.NameIdentifier,login.Username),
        //        new (ClaimTypes.Email,login.Email)
        //        })
        //    };
        //    var securitytoken = tokenhandler.CreateToken(tokendescriptor);
        //    var accesstoken = tokenhandler.WriteToken(securitytoken);
        //    return Ok(accesstoken);
        //}
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult<string>> RegisterAUser([FromBody] UserRegisteration userregister)
        {
           var result= await _authservice.RegisterAUser(userregister);
            if (result == true)
            {
                UserLogin userlogin = userregister.Adapt<UserLogin>();
                var tokenstring =await _authservice.GetJwtToken(userlogin);
                return StatusCode(StatusCodes.Status200OK, tokenstring);
            }
            return BadRequest();
        }
        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult<string>> LoginAUser([FromBody] UserLogin userlogin)
        {
            var result = await _authservice.LoginAUser(userlogin);
            if(result==true)
            {
                var tokenstring = await _authservice.GetJwtToken(userlogin);
                return StatusCode(StatusCodes.Status200OK, tokenstring);
            }
            return BadRequest();
        }
    }
}
