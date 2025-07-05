using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
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
        [HttpPut("ChangeMyPassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var currentUserId = User.FindFirst("TheCallerID")?.Value;

            var result = await _employeeLogicService.ChangePassword(currentUserId, request);

            if (!result.Succeeded)
            {
                if (result.NewPasswordCause)
                {
                    return BadRequest(new
                    {
                        Error = result.Errors,
                        Message = result.Messages,
                        PasswordScore = result.Score,
                        PasswordStrength = result.Strength,
                        Suggestions = result.Suggestions
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Error = result.Errors,
                        Message = result.Messages,
                    });
                }

            }
            return Ok(new
            {
                Message = result.Messages,
            });
        }
        }
    }

