using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

using Proz_WebApi.Models.DesktopModels.DTO.LoginHistoryDTOs;

namespace Proz_WebApi.Services.DesktopServices
{
    public class HRLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;

        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AdminLogicService> _logger;


        public HRLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager, ApplicationDbContext_Desktop dbcontext, ILogger<AdminLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }

     

    


    }
}
