using System.ComponentModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.Dto.Admin;
using Proz_WebApi.Services;

namespace Proz_WebApi.Controllers
{
    [ApiController]
    [Route("Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "AdminOnly")]
    public class AdminLogicController : ControllerBase
    {
        private readonly AdminLogicService _adminlogicservice;

        public AdminLogicController(AdminLogicService adminlogicservice)
        {

            _adminlogicservice = adminlogicservice;
          
        }


        [HttpPut("Users/AssignRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] RoleChangeRequestAdmin request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UpdateRoles(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new
                {
                    Error = result.Errors,
                    Message = result.Messages,
                    NumberOfFailedProcesses = result.FailedCount,
                    NumberOfSuccessProcesses = result.SuccessCount
                });

            }
            return Ok(new
            {   Error=result.Errors,
                Message = result.Messages,
                NumberOfFailedProcesses = result.FailedCount,
                NumberOfSuccessProcesses = result.SuccessCount
            });
        }

        [HttpGet("Users/GetUsers")]
        public async Task<ActionResult<IEnumerable<ReturnUsersWithRolesAdminDto>>> GetUsers()
        {
            var response = await _adminlogicservice.GetUsers();
            if (response == null || !response.Any())
            {
                return BadRequest("The system is empty from users");
            }
            return Ok(response);
        }




       

        [HttpDelete("Users/DeleteAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteUsers([FromBody] RequestAccountsRemoveAdmin request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            if(!request.UserIDs.Any()||request.UserIDs==null)
            {
               return BadRequest("Please select a user atleast");
            }

            var result = await _adminlogicservice.DeleteAccounts(request.UserIDs, currentUserId);


            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Error = result.Errors,
                    Message = result.Messages,
                    NumberOfFailedProcesses = result.FailedCount,
                    NumberOfSuccessProcesses = result.SuccessCount
                });
            }
                return Ok(new
                {
                    Message = result.Messages,
                    NumberOfFailedProcesses = result.FailedCount,
                    NumberOfSuccessProcesses = result.SuccessCount
                });
            


        }

    }
}
