using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proz_WebApi.Helpers_Types;
//using Proz_WebApi.Models.DesktopModels.Dto.DepartmentManager;
using Proz_WebApi.Services.DesktopServices;

namespace Proz_WebApi.Controllers.DesktopControllers
{
    [Route("DepartmentManager")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "DepartmentManager")]
    public class DepartmentManagerLogicController : ControllerBase
    {
        private readonly DepartmentManagerLogicService _departmentManagerLogicService;

        public DepartmentManagerLogicController(DepartmentManagerLogicService departmentManagerLogicService)
        {
            _departmentManagerLogicService = departmentManagerLogicService;
        }

      





        
    }
}
