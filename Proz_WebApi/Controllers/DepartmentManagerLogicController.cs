using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.Dto;
using Proz_WebApi.Models.Dto.Admin;
using Proz_WebApi.Models.Dto.DepartmentManager;
using Proz_WebApi.Services;

namespace Proz_WebApi.Controllers
{
    [Route("DepartmentManager")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentManagement")]
    public class DepartmentManagerLogicController : ControllerBase
    {
        private readonly DepartmentManagerLogicService _departmentManagerLogicService;

        public DepartmentManagerLogicController(DepartmentManagerLogicService departmentManagerLogicService)
        {
            _departmentManagerLogicService = departmentManagerLogicService;
        }

        [HttpPut("Users/AssignRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] RoleChangeRequestDM request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.UpdateRoles(currentUserId ,request);

            if (result.Succeeded)
                return Ok("The roles have been assigned successfully!");
            return BadRequest(result.Errors);
        }

        [HttpDelete("Users/DeleteAccounts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentManagement")]
        public async Task<IActionResult> DeleteDepartmentEmployees([FromBody] RequestAccountsRemoveDM request)

        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Authentication required to perform the operation");
            }

            if (!request.UserIDs.Any() || request.UserIDs == null)
            {
                return BadRequest("Please select a user atleast");
            }

            var result = await _departmentManagerLogicService.DeleteUsers(request.UserIDs, currentUserId);


            if (result.Succeeded) 
            { 
                return BadRequest(new
                {
                    Errors = result.Errors,
                    Message = result.Messages,
                    NumberOfFailedProcesses = result.FailedCount,
                    NumberOfSuccessProcesses = result.SuccessCount
                });
            }
            return Ok(new
            {   Errors=result.Errors,
                Message = result.Messages,
                NumberOfFailedProcesses = result.FailedCount,
                NumberOfSuccessProcesses = result.SuccessCount
            });





        }
    }
}
