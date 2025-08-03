using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.DTO.LoginHistoryDTOs;

namespace Proz_WebApi.Services.DesktopServices
{
    public class LoginHistoryService
    {
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly ILogger<AdminLogicService> _logger;
        public LoginHistoryService(ApplicationDbContext_Desktop dbcontext, UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<LoginHistoryDto>> GetRecentLoginHistory(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return null;
            }
            return await _dbcontext.LoginHistoryTable
                .Where(h => h.ExtendedIdentityUsersDesktop_FK == userId)
                .OrderByDescending(h => h.LoggedAt)
                .Take(250)
                .Select(h => new LoginHistoryDto
                {
                    LoggedAt = h.LoggedAt,
                  
                })
                .ToListAsync();
        }
    }
}
