using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;


//using Proz_WebApi.Models.DesktopModels.Dto.DepartmentManager;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("DM")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentManager")]
    public class DepartmentManagerLogicController : ControllerBase
    {
        private readonly DepartmentManagerLogicService _departmentManagerLogicService;

        public DepartmentManagerLogicController(DepartmentManagerLogicService departmentManagerLogicService)
        {
            _departmentManagerLogicService = departmentManagerLogicService;
        }


        [HttpGet("Departments/Get")]
        public async Task<IActionResult> GetDepartments()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnMyDepartments(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpGet("Feedbacks/Get")]
        public async Task<IActionResult> GetFeedback(ReturnFeedbacksRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnFeedbacksOfADepartment(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("FeedbacksAnswer/Add")]
        public async Task<IActionResult> AddFeedbackAnswer(AddAnAnswerForAFeedbackRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.CreateAnAnswerForAFeedback(currentUserId, request);

            if (result.Success==false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("FinishedFeedbacks/Get")]
        public async Task<IActionResult> GetFinishedFeedback(ReturnFinishedFeedbacksRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnFinishedFeedbacksOfADepartment(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpGet("LeaveRequests/Get")]
        public async Task<IActionResult> GetLeaveRequests(ReturnMyEmployeesLeaveRequests_Request_ request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnLeaveRequestsOfADepartment(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("LeaveRequest/SendAnswer")]
        public async Task<IActionResult> GiveAnswerToALeaveRequest(LeaveRequestAcceptRejectRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.CreateAnAnswerForALeaveRequest(currentUserId, request);

            if (result.Success == false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("CompletedLeaveRequests/Get")]
        public async Task<IActionResult> GetCompletedLeaveRequests(ReturnFinishedLeaveRequestsRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnFinishedLeaveRequestsOfADepartment(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Employees/Get")]
        public async Task<IActionResult> GetMyEmployees(ReturnPerformanceRecordsRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.ReturnEmployeesWithIDsDepartment(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("Performance/SendPerformance")]
        public async Task<IActionResult> MakeAPerformance(SubmitPerformanceAnswerRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _departmentManagerLogicService.CreateAnAnswerForPerformance(currentUserId, request);

            if (result.Success == false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


    }
}
