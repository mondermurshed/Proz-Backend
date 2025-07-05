using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
//using Proz_WebApi.Models.DesktopModels.Dto.DepartmentManager;
using Zxcvbn;

namespace Proz_WebApi.Services.DesktopServices
{
    public class DepartmentManagerLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<DepartmentManagerLogicService> _logger;


        public DepartmentManagerLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager, ApplicationDbContext_Desktop dbcontext, ILogger<DepartmentManagerLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }
  
       
   
    }
}
