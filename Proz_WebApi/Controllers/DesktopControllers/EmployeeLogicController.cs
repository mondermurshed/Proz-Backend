using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;
using Proz_WebApi.Models.DesktopModels.DTO.Employee;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("Employee")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Employee")]
    public class EmployeeLogicController : ControllerBase
    {
        private readonly EmployeeLogicService _employeeLogicService;
    public EmployeeLogicController(EmployeeLogicService employeeLogicService)
        {
          _employeeLogicService= employeeLogicService;
        }

        [HttpGet("Feedbacks/GetFeedbackTypes")]
        public async Task<IActionResult> GetFeedbackTypes()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.GetFeedbackTypes(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("Feedbacks/RequestANewFeedback")]
        public async Task<IActionResult> CreateAFeedbackType([FromBody] CreateANewFeedbackRequest_Request request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.CreateANewfeedbackRequest(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new CreateANewFeedbackRequest_Response
                {
                    Errors = result.Errors[0],
                    Message = null

                });

            }
            return Ok(new CreateANewFeedbackRequest_Response
            {
                Errors = null,
                Message = result.Messages[0]

            });
        }



        [HttpGet("Feedbacks/ReturnMyFeedbacksInformation")]
        public async Task<IActionResult> ReturnFeedbackList()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.GetMyFeedbacks(currentUserId);
          
            if (result==null)
            {

                return BadRequest();
               
            }
            return Ok(result);

        }


        [HttpDelete("Feedbacks/Remove")]
        public async Task<IActionResult> RemoveMyFeedback(RemoveMyFeedbackRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.RemoveMyFeedback(currentUserId,request);

            if (result.Success==false)
            {

                return BadRequest(result);

            }
            return Ok(result);

        }


        [HttpPost("LeaveRequest/Add")]
        public async Task<IActionResult> CreateALeaveRequest([FromBody] CreateANewLeaveRequest_Request request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.CreateANewLeaveRequest(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new CreateANewLeaveRequest_Response_
                {
                    Errors = result.Errors[0],
                    Message = null

                });

            }
            return Ok(new CreateANewLeaveRequest_Response_
            {
                Errors = null,
                Message = result.Messages[0]

            });
        }

        [HttpGet("LeaveRequest/Get")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.GetMyLeaveRequests(currentUserId);

            if (result==null || !result.Any())
            {

                return BadRequest();

            }
            return Ok(result);




        }

        [HttpDelete("LeaveRequest/Remove")]
        public async Task<IActionResult> RemoveMyLeaveRequest(RemoveMyLeaveRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.RemoveMyLeaveRequest(currentUserId, request);

            if (result.Success == false)
            {

                return BadRequest(result);

            }
            return Ok(result);

        }


        [HttpPost("LeaveRequest/AddFinalAnswer")]
        public async Task<IActionResult> AddLeaveRequestAnswer(AgreeOnLeaveRequestDecisionRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.AgreeOnLeaveRequestRules(currentUserId, request);

            if (result.Success == false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("PerformanceRecords/Get")]
        public async Task<IActionResult> GetMyPerformanceRecords(ReturnPerformanceRecordsListRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _employeeLogicService.GetMyPerformanceRecords(currentUserId,request);

            if (result == null || !result.Any())
            {

                return BadRequest();

            }
            return Ok(result);




        }
    }



    }

