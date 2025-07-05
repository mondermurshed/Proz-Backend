using System.ComponentModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [ApiController]
    [Route("Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] This code is for Authentication only! meaning it will check if the user has a valid (everything is correct) token then it will make him enter this controller, you might see [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "AdminOnly")] which is for both Authentication and Authorization meaning if he has a valid token + he didn't has any problem with the policy then make him enter. 
    public class AdminLogicController : ControllerBase
    {
        private readonly AdminLogicService _adminlogicservice;

        public AdminLogicController(AdminLogicService adminlogicservice)
        {

            _adminlogicservice = adminlogicservice;

        }


     

        [HttpGet("Users/GetUsers")]
        public async Task<ActionResult<IEnumerable<ReturnUsersWithRolesAdminDto>>> GetUsers()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            var response = await _adminlogicservice.GetUsers();
            if (response == null || !response.Any())
            {
                return BadRequest("The system is empty from users");
            }
            return Ok(response);
        }







    }
}
