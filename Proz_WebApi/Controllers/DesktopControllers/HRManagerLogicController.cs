using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;
using Proz_WebApi.Models.DesktopModels.DTO.HRManager;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("HRM")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "HRManager")]
    public class HRManagerLogicController : ControllerBase
    {


        private readonly HRLogicService _HRlogicservice;

        public HRManagerLogicController(HRLogicService HRlogicservice)
        {

            _HRlogicservice = HRlogicservice;

        }

        [HttpGet("LeaveRequests/Get")]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _HRlogicservice.ReturnLeaveRequests(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("LeaveRequest/SendAnswer")]
        public async Task<IActionResult> GiveAnswerToALeaveRequest(LeaveRequestAcceptRejectHRMRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _HRlogicservice.CreateAnAnswerForALeaveRequest(currentUserId, request);

            if (result.Success == false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpGet("CompletedLeaveRequests/Get")]
        public async Task<IActionResult> GetCompletedLeaveRequests()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _HRlogicservice.ReturnFinishedLeaveRequestsOfADepartment(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }




    }
}
