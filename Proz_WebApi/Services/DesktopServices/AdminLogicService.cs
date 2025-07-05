using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System.Transactions;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Zxcvbn;

namespace Proz_WebApi.Services.DesktopServices
{
    public class AdminLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
      
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AdminLogicService> _logger;


        public AdminLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager, ApplicationDbContext_Desktop dbcontext, ILogger<AdminLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }
       
        //-------------------------------------------------------------------------------------------------------   
        public async Task<IEnumerable<ReturnUsersWithRolesAdminDto>> GetUsers()
        {
            return await _userManager.Users
                           .Include(u => u.UserRolesNA)
                           .ThenInclude(ur => ur.RoleNA)
                           // Project to DTO using Mapster
                           .ProjectToType<ReturnUsersWithRolesAdminDto>()
                           .ToListAsync();
        }
        //-------------------------------------------------------------------------------------------------------   
       
    }
}
