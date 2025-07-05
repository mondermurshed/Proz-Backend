using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("LoginHistory")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "HRManager")]
    public class LoginHistortyController : ControllerBase
    {
        private readonly LoginHistoryService _loginHistoryService;
        public LoginHistortyController(LoginHistoryService loginHistoryService)
        {
            _loginHistoryService=loginHistoryService;
        }

        [HttpGet("Get/{userId}")]
        public async Task<IActionResult> GetLoginHistory(Guid userId)
        {
            var requester = User.FindFirst("TheCallerID")?.Value;
            if (string.IsNullOrEmpty(requester))
            {

                return Unauthorized("The Requester is unknown");
            }
            var result = await _loginHistoryService.GetRecentLoginHistory(userId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
