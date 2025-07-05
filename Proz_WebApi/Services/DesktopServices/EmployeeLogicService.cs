using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Proz_WebApi.Data;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.DTO.Employee;

namespace Proz_WebApi.Services.DesktopServices
{
    public class EmployeeLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AdminLogicService> _logger;

        public EmployeeLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager, ApplicationDbContext_Desktop dbcontext, ILogger<AdminLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }

        public async Task<FinalResultWithPasswordCheckingInfo> ChangePassword(string? userId, ChangePasswordDto dto)
        {
            var result = new FinalResultWithPasswordCheckingInfo();
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                if (string.IsNullOrEmpty(userId))
                {
                    result.Succeeded = false;
                    result.Errors.Add("User not found.");
                    return result;
                }
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    result.Succeeded = false;
                    result.Errors.Add("User not found.");
                    return result;
                }

                var passwordValidation = PasswordChecker.ValidatePassword(dto.NewPassword, user.Email, user.UserName);

                if (!passwordValidation.IsValid)
                {
                    result.Succeeded = false;
                    result.Errors.Add(passwordValidation.Message);
                    result.Suggestions.AddRange(passwordValidation.Suggestions);
                    result.Score = passwordValidation.Score;
                    result.CrackTime = passwordValidation.CrackTime;
                    result.NewPasswordCause = true;
                    return result;
                }

                var changeResult = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

                if (!changeResult.Succeeded)
                {
                    result.Succeeded = false;
                    result.Errors.AddRange(changeResult.Errors.Select(e => e.Description));
                    return result;
                }

                result.Succeeded = true;
                result.Messages.Add("Your password has been changed successfully.");
                scope.Complete();
                return result;
            }
        }
    }
}
