using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Models.DesktopModels.DTO.HRManager;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("HRManager")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "HRManager")]
    public class HRManagerLogicController : ControllerBase
    {


        private readonly HRLogicService _adminlogicservice;

        public HRManagerLogicController(HRLogicService adminlogicservice)
        {

            _adminlogicservice = adminlogicservice;

        }
        [HttpPut("Users/AssignRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] RoleUpdateDTO request)
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
                    NumberOfSuccessProcesses = result.SuccessCount,
                    NumberOfSkippedProcesses = result.SkippedCount
                });

            }
            return Ok(new
            {
                Error = result.Errors,
                Message = result.Messages,
                NumberOfFailedProcesses = result.FailedCount,
                NumberOfSuccessProcesses = result.SuccessCount,
                NumberOfSkippedProcesses = result.SkippedCount
            });
        }



        [HttpDelete("Users/DeleteAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteUsers([FromBody] RemoveUsersDTO request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            if (!request.UserIDs.Any() || request.UserIDs == null)
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
                    NumberOfSuccessProcesses = result.SuccessCount,
                    NumberOfSkippedProcesses = result.SkippedCount
                });
            }
            return Ok(new
            {
                Error = result.Errors,
                Message = result.Messages,
                NumberOfFailedProcesses = result.FailedCount,
                NumberOfSuccessProcesses = result.SuccessCount,
                NumberOfSkippedProcesses = result.SkippedCount
            });



        }
    }
}
