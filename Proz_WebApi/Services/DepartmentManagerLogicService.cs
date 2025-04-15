using System.Data;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models;
using Proz_WebApi.Models.Dto.DepartmentManager;
using Zxcvbn;

namespace Proz_WebApi.Services
{
    public class DepartmentManagerLogicService
    {
        private readonly UserManager<ExtendedIdentityUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTOptions _jwtoption;
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<DepartmentManagerLogicService> _logger;


        public DepartmentManagerLogicService(UserManager<ExtendedIdentityUsers> userManager, RoleManager<IdentityRole> roleManager, JWTOptions jwtoption, ApplicationDbContext dbcontext, ILogger<DepartmentManagerLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtoption = jwtoption;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }
        public async Task<FinalResult> UpdateRoles(string requesterid, RoleChangeRequestDM request)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();

            try
            {

                var requster = await _userManager.FindByIdAsync(requesterid);
                if (requster == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("The requester it unknown. the operation is stopped from performing");
                    
                    return finalresult;
                }
                if (!await _roleManager.RoleExistsAsync(request.NewRole))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add($"The {request.NewRole} role that you want to assign is not a part of the roles inside the system. Please select a valid role");
                    return finalresult;
                }
                var allowedRoles = new List<string> { AppRoles.User, AppRoles.Employee };
                if (!allowedRoles.Contains(request.NewRole))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add($"Not authorized to assign {request.NewRole} role. Try to select a lower role like {string.Join(',', allowedRoles, 1, allowedRoles.Count-1)} ");
                }
                var userIds = request.UsersIDs.Distinct().ToList();

                var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                var missingUsers = userIds.Except(users.Select(u => u.Id)).ToList();
                if (missingUsers.Any())
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add($"Users with the IDs {string.Join(',', missingUsers, 1, missingUsers.Count - 1)} are missing in the database. Please select valid users");
                    return finalresult;
                }
                foreach (var user in users) //the department manager can make any user to be an emplyee but he can only make an employee to be a user again only if this employee is inside his department.
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (currentRoles.Any(r=>!allowedRoles.Contains(r)))
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"User {user.UserName} got high roles. Can't operate the mission");
                        continue;
                    }

                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"Got some errors while removing the '{currentRoles.FirstOrDefault()}' role from the user {user.UserName} because {string.Join(',', removeResult.Errors.Select(e => e.Description),1, removeResult.Errors.Select(e => e.Description).Count()-1)}");
                        continue;
                    }

                    var addResult = await _userManager.AddToRoleAsync(user, request.NewRole);
                    if (addResult.Succeeded)
                    {
                        finalresult.SuccessCount++;
                    }
                    else
                    {
                        finalresult.FailedCount++;
                        var assigningOldRoles = await _userManager.AddToRolesAsync(user, currentRoles);
                        if (assigningOldRoles.Succeeded)
                        {
                            finalresult.Errors.Add($"Got some errors while assigning the new role '{request.NewRole}' to the user {user.UserName} because {string.Join(',', addResult.Errors.Select(e => e.Description), 1, addResult.Errors.Select(e => e.Description).Count() - 1)}. The old roles of the user are restored");
                            continue;
                        }
                       else
                        {
                            finalresult.Errors.Add($"Got some errors while assigning the new role '{request.NewRole}' to the user {user.UserName} because {string.Join(',', addResult.Errors.Select(e => e.Description), 1, addResult.Errors.Select(e => e.Description).Count() - 1)}. Another error happened when trying to restore the old ones as well.");
                            continue;
                        }
                       
                    }
                }
                if (finalresult.SuccessCount > 0)
                {
                    await transaction.CommitAsync();
                    finalresult.Succeeded = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                }
                return finalresult;


            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Department role update failed");
                finalresult.Errors.Clear();
                finalresult.Errors.Add($"Failed to process role updates {ex.Message}");
                finalresult.Succeeded = false;
                return finalresult;
            }
        }
//----------------------------------------------------------------------------------------------
        public async Task<FinalResult> DeleteUsers(List<string> userIDs, string currentManagerId)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();
            

            try
            {
                // Validate requesting manager exists
                var currentManager = await _userManager.FindByIdAsync(currentManagerId);
                if (currentManager == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The requester is unknown. Can't perform any action to the system.");
                    return finalresult;
                  
                }

                // Get distinct IDs and batch-fetch users
                var distinctIds = userIDs.Distinct().ToList();
                var users = await _userManager.Users
                    .Where(u => distinctIds.Contains(u.Id))
                    .Include(u => u.UserRoles)
                    .ToListAsync();
                
                var userMap = users.ToDictionary(u => u.Id);
                var processedIds = new HashSet<string>();
                

                foreach (var userId in distinctIds)
                {
                    if (!userMap.TryGetValue(userId, out var user))
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"The user with the ID {userId} is not existing inside the database.");
                        continue;
                    }

                    if (processedIds.Contains(userId)) continue;
                    processedIds.Add(userId);

                    // Role validation
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(AppRoles.Admin) || roles.Contains(AppRoles.HRManager) || roles.Contains(AppRoles.DepartmentManager))
                    {
                        finalresult.FailedCount++;

                        //var username = await _userManager.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync();
                        finalresult.Errors.Add($"The user {user.UserName} has a high roles. We can't apply any role to it.");
                        continue;
                    }

                    // Future department check placeholder
                    // if (user.Department != currentManager.Department) 
                    // {
                    //     messageBuilder.AppendLine($"• {userId} not in your department");
                    //     continue;
                    // }

                    // Deletion attempt
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (deleteResult.Succeeded)
                    {
                        finalresult.SuccessCount++;
                        finalresult.Messages.Add($"The user {user.UserName} was deleted successfully.");
                    }
                    else
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"We couldn't delete the user {user.UserName} because {string.Join(',', deleteResult.Errors.Select(e => e.Description))}");
                        
                    }
                }

                // Transaction handling
                if (finalresult.SuccessCount > 0)
                {
                    await transaction.CommitAsync();
                    finalresult.Succeeded = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                }


                return finalresult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Department bulk delete failed for {Count} users", userIDs.Count);
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Errors.Add($"Something went wrong while doing the operation {ex.Message}");
                return finalresult;
            }
        }
    }
}
