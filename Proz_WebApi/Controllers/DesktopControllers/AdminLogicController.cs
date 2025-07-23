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



    }
}
