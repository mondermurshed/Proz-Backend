using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
     


       
    }
}
