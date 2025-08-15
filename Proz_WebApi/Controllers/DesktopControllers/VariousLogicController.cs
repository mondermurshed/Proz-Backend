using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [ApiController]
    [Route("General")]
    public class VariousLogicController : ControllerBase
    {
        private readonly AdminLogicService _adminlogicservice;
        private readonly DepartmentManagerLogicService _DMlogicservice;

        public VariousLogicController(AdminLogicService adminlogicservice, DepartmentManagerLogicService DMlogicservice)
        {

            _adminlogicservice = adminlogicservice;
            _DMlogicservice = DMlogicservice;
        }


        [HttpGet("Company/Name/Get")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AllExceptUsers")]
        public async Task<IActionResult> GetCompanyName()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetCompanyName(currentUserId);

            if (result.CompanyName == null)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Users/Data/PersonalData")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AllExceptUsers")]
        public async Task<IActionResult> GetPersonalData()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllPersonalData(currentUserId);

            if (result == null)
            {

                return BadRequest();


            }
            return Ok(result);

        }

        [HttpGet("Users/Role/Get")]
        //[AllowAnonymous]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AllUsers")]
        public async Task<IActionResult> GetRoleName()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetRole(currentUserId);

            if (result == null)
            {

                return BadRequest();


            }
            return Ok(result);

        }
        
        [HttpGet("Users/MyLoginHistory")]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EDH")]
        public async Task<IActionResult> GetLoginHistoryOfMine()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _DMlogicservice.ReturnMyLoginHistoty(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

    }
}
