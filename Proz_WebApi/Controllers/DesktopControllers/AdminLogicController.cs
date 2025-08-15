using System.ComponentModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using Proz_WebApi.Models.DesktopModels.DTO;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Auth;
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
    
        [Route("GettingStatedFirstStage")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> GettingStartedStepOne([FromBody] GettingStartedStageOneDTO userregister)
        {


            var validation = PasswordChecker.ValidatePassword(
            userregister.AdminPassword,
            userregister.AdminEmail,
            userregister.AdminUsername

        );
            var MessageService = new List<string>();
            if (!validation.IsValid)
            {
                MessageService.Add(validation.Message);
                return BadRequest(new RegisterResponse
                {
                    Message = MessageService,
                    Error = null,
                    Score = validation.Score,
                    Strength = validation.Strength,
                    CrackTime = validation.CrackTime,
                    Suggestions = validation.Suggestions.ToList(),
                    PasswordCause = true


                });
            }



            var result = await _adminlogicservice.InitializeSystemAsyncStepOne(userregister);
            if (result.Succeeded)
            {
                MessageService.Add($"A verification code has been sent successfully to your email service, please re type the code here. The code validity period is one minute");

                return Ok(new RegisterResponse
                {
                    Message = MessageService,
                    Error = null,
                    CrackTime = null,
                    Suggestions = null,
                    Strength = validation.Strength,
                    Score = validation.Score
                });
            }
            else
            {

                return BadRequest(new RegisterResponse
                {
                    Message = result.Messages,
                    Error = result.Errors,
                    Suggestions = null,
                    CrackTime = null,
                    Strength = null

                });
            }
        }
       
        [Route("GettingStatedSecondStage")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> RegisterAUserStageTwo([FromBody] GettingStartedStageTwoDTO userregister)
        {
            var result = await _adminlogicservice.InitializeSystemAsyncStepTwo(userregister);
            var MessageService = new List<string>();
            if (result.Succeeded)
            {
                MessageService.Add("Your account has been successfully created! You can now log in as the Admin!");
                return Ok(new AuthResponseDTO
                {
                    Message = MessageService,
                    Error = null
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


        [Route("ResendCodeAdmin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> ResendVerificationCodeAsync([FromBody] ResendVerificationCodeDTO resendcode)
        {
            var result = await _adminlogicservice.InitializeSystemAsyncResendData(resendcode);
            if (result.Succeeded)
            {

                return Ok(new AuthResponseDTO
                {
                    Message = result.Messages,
                    Error = null

                });
            }
            else
            {
                return BadRequest(new AuthResponseDTO
                {
                    Message = result.Messages,
                    Error = result.Errors


                });
            }
        }

        [Route("CheckGettingStatus")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> CheckingGettingStatus()
        {
            var result = await _adminlogicservice.CheckingGettingStatusService();
          
            if (result.Succeeded)
            {

                return Ok();
             
              
            }
            else
            {
                return BadRequest();
                
     


                
            }
        }


        [HttpPut("Users/AssignRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserInformationClass request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UpdateRoles(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new RoleUpdateResponse
                {
                    Errors = result.Errors,
                    Message =null
                   
                });

            }
            return Ok(new RoleUpdateResponse
            {
                Errors =null,
                Message = result.Messages
            
            });
        }



        [HttpGet("Users/GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ReturnUsersWithRolesAdminDto>>> GetAllUsers()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            var response = await _adminlogicservice.GetAllUsers(currentUserId);
            if (response == null || !response.Any())
            {
                return BadRequest("The system is empty from users");
            }
            return Ok(response);
        }



        [HttpDelete("Users/DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteAUser([FromBody] RemoveUserDTO request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            if (request.UserID==null || string.IsNullOrEmpty(request.UserID.ToString()))
            {
                return BadRequest("Please select a user to be deleted");
            }

            var result = await _adminlogicservice.DeleteAccount(request.UserID, currentUserId);


            if (!result.Succeeded)
            {
                return BadRequest(new AccountDeleteResponse
                {
                  Message=null,
                  Errors = result.Errors
                });
            }
            return Ok(new AccountDeleteResponse
            {
              Errors = null,
              Message=result.Messages
            });



        }


        [HttpPost("Feedbacks/CreateFeedbackType")]
        public async Task<IActionResult> CreateAFeedbackType([FromBody] CreateAFeedbackTypeRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.CreateANewfeedbackType(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new CreateAFeedbackTypeResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new CreateAFeedbackTypeResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }

        [HttpGet("Feedbacks/GetFeedbackTypes")]
        public async Task<IActionResult> GetFeedbackTypes()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetFeedbackTypes(currentUserId);

            if (result==null || result.Count()==0)
            {

                return BadRequest(result);
     

            }
            return Ok(result);
   
        }


        [HttpDelete("Feedbacks/DeleteAFeedbackType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteAFeedbackType([FromBody] RemoveFeedbackTypeDTO request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }
            if (request.FeedbackID == null || string.IsNullOrEmpty(request.FeedbackID.ToString()))
            {
                return BadRequest("Please select a user to be deleted");
            }

            var result = await _adminlogicservice.RemoveAFeedbackType(request, currentUserId);


            if (!result.Succeeded)
            {
                return BadRequest(new RemoveFeedbackTypeResponse
                {
                    Message = null,
                    Errors = result.Errors
                });
            }
            return Ok(new RemoveFeedbackTypeResponse
            {
                Errors = null,
                Message = result.Messages
            });



        }

        [HttpGet("Users/GetAllDepartmentManagers")]
        public async Task<IActionResult> GetDepartmentManagers()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllDepartmentManagers(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPost("Department/Create")]
        public async Task<IActionResult> CreateANewDepartment([FromBody] DepartmentCreatingRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.CreateANewDepartment(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new CreateANewDepartmentResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new CreateANewDepartmentResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }

        [HttpGet("Users/GetAllDepartmentsWithItsManagers")]
        public async Task<IActionResult> GetAllDepartmentsAlongWithItsManagers()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllDepartmentsAlongWithItsManagers(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPatch("Department/UnassigningManager")]
        public async Task<IActionResult> UnassignManagerFromDepartment([FromBody] UnassignAManagerFromADepartmentRequest request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UnassignAManagerfromADepartment(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new UnassignAManagerFromADepartmentResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new UnassignAManagerFromADepartmentResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }


        [HttpDelete("Department/Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteADepartment([FromBody] RemoveADepartmentRequest request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.RemoveADepartment(request, currentUserId);


            if (!result.Succeeded && result.NeedsApproval==true)
            {
                return BadRequest(new RemoveADepartmentResponse
                {
                    Message = null,
                    Errors = result.Errors,
                    NeedesAdminApproval = true
                });
            }
            else if ((!result.Succeeded && result.NeedsApproval == false))
            {
                return BadRequest(new RemoveADepartmentResponse
                {
                    Message = null,
                    Errors = result.Errors,
                    NeedesAdminApproval = false
                });
            }

            return Ok(new RemoveADepartmentResponse
            {
                Errors = null,
                Message = result.Messages
            });



        }

        [HttpGet("Department/Get")]
        public async Task<IActionResult> GetDepartments()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllDepartments(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPatch("Department/AssigningManager")]
        public async Task<IActionResult> AssignManagerTOTheDepartment([FromBody] AssignManagerToADepartmentRequest request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.AssignManagerToADepartment(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new AssignManagerToADepartmentResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new AssignManagerToADepartmentResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }

        [HttpGet("Users/GetLoginHistory/My")]
        public async Task<IActionResult> GetLoginHistoryOfMine(ReturnLoginHistoryForMyselfRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.ReturnLoginHistoryOfMine(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Users/GetLoginHistory/Manager")]
        public async Task<IActionResult> GetLoginHistoryOfManager(ReturnLoginHistoryForManagerRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.ReturnLoginHistoryOfManager(currentUserId, request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Users/AllManagers/Get")]
        public async Task<IActionResult> GetAllManagers()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllManagers(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpGet("Users/AllManagersAndAdmins/Get")]
        public async Task<IActionResult> GetAllManagersAndAdmins()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllManagersAndAdmins(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Users/Logs/Get")]
        public async Task<IActionResult> GetLogs(GetLogsForAPersonRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetLogsForAPerson(currentUserId,request);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Users/Employees/Get")]
        public async Task<IActionResult> GetEmployees()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllEmployees(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpGet("Department/GetAll")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetDepartmentsForEmployees(currentUserId);

            if (result == null || result.Count() == 0)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpPatch("Users/AssigningEmployee")]
        public async Task<IActionResult> AssignEmployeeTOTheDepartment([FromBody] AssignEmployeeToADepartmentRequest request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.AssignEmployeeToADepartment(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new AssignEmployeeToADepartmentResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new AssignEmployeeToADepartmentResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }

        [HttpGet("Users/Employee/GetDepartmentName")]
        public async Task<IActionResult> GetDepartmentNameEmployee(GetDepartmentOfEmployeeRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetDepartmentOFAnEmployees(currentUserId, request);

            if (result.GotDepartment == false)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }


        [HttpPatch("Users/UnassigningEmployee")]
        public async Task<IActionResult> UnAssignEmployee([FromBody] UnassignEmployeeToADepartmentRequest request)
        {


            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UnassignEmployeeFromADepartment(currentUserId, request);

            if (!result.Succeeded)
            {

                return BadRequest(new UnassignEmployeeToADepartmentResponse
                {
                    Errors = result.Errors,
                    Message = null

                });

            }
            return Ok(new UnassignEmployeeToADepartmentResponse
            {
                Errors = null,
                Message = result.Messages

            });
        }

        [HttpPatch("Company/Name/Update")]
   
        public async Task<IActionResult> UpdateTheCompanyName(UpdateCompanyNameRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UpdateCompanyName(currentUserId,request );

            if (result==null)
            {

                return BadRequest();


            }
            return Ok(result);

        }

        [HttpGet("System/Roles/Get")]
        public async Task<IActionResult> GetSystemRoles()
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.GetAllSystemRoles(currentUserId);

            if (result == null)
            {

                return BadRequest(result);


            }
            return Ok(result);

        }

        [HttpPatch("System/Roles/Color/Update")]
        public async Task<IActionResult> UpdateRoleColor(RoleColorChangeRequest request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {

                return Unauthorized("The Requester is unknown");
            }

            var result = await _adminlogicservice.UpdateRoleColor(currentUserId,request);

            if (result == null)
            {

                return BadRequest();


            }
            return Ok();

        }

    }
}
