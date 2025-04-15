using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models;
using Proz_WebApi.Models.Dto.Admin;
using Zxcvbn;

namespace Proz_WebApi.Services
{
    public class AdminLogicService
    {
        private readonly UserManager<ExtendedIdentityUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTOptions _jwtoption;
        private readonly ApplicationDbContext _dbcontext;
        private readonly ILogger<AdminLogicService> _logger;


        public AdminLogicService(UserManager<ExtendedIdentityUsers> userManager, RoleManager<IdentityRole> roleManager, JWTOptions jwtoption, ApplicationDbContext dbcontext, ILogger<AdminLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtoption = jwtoption;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }
        public async Task<FinalResult> UpdateRoles(string requesterId, RoleChangeRequestAdmin request)


        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();

            try
            {
               
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {
                    finalresult.Errors.Clear();
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The requester is unknown. Couldn't done the operation");
                    return finalresult;
                }
                // Get all unique user IDs and roles from request
                var userIds = request.UserInformation.Select(rc => rc.UserId).Distinct().ToList();
                var allRoles = request.UserInformation
                    .SelectMany(rc => rc.NewRoles)
                    .Distinct()
                    .ToList();

                // Batch fetch users and roles
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                var existingRoles = await _roleManager.Roles
                    .Where(r => allRoles.Contains(r.Name))
                    .Select(r => r.Name)
                    .ToListAsync();

                // Validate users and roles
                var missingUsers = userIds.Except(users.Select(u => u.Id)).ToList();
                var missingRoles = allRoles.Except(existingRoles).ToList();

                if (missingUsers.Any() || missingRoles.Any())
                {
                    finalresult.Errors.Clear();
                    if (missingUsers.Any())
                     finalresult.Errors.Add($"The users with these IDs {string.Join(", ", missingUsers)} are missing inside the database");
                    
                    //var username = await _userManager.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync();
                    if (missingRoles.Any())
                     finalresult.Errors.Add($"These roles {string.Join(", ", missingRoles)} are missing inside the database");

                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                    finalresult.Messages.Add("Try to select only valid users and then try again. No changes was happened to the database");
                    return finalresult;
                }
              
                // Process each user's role changes
                foreach (var UserInformation in request.UserInformation)
                {
                    var user = users.First(u => u.Id == UserInformation.UserId);
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    // Remove existing roles if replacing
                    if (UserInformation.ReplaceExisting && currentRoles.Any())
                    {
                        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        if (!removeResult.Succeeded)
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"Something went wrong for the user {user.UserName} because {string.Join(", ", removeResult.Errors.Select(e => e.Description), 1, removeResult.Errors.Count() - 1)} No operation was done for this user.");
                            continue;
                        }
                   
                    }

                    // Determine roles to add
                    List<string> rolesToAdd=new List<string>();
                    if (UserInformation.ReplaceExisting)
                    {
                        rolesToAdd = UserInformation.NewRoles;
                    }
                    else
                    {
                        rolesToAdd = UserInformation.NewRoles.Except(currentRoles).ToList();
                    }

                    if (rolesToAdd.Any())
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                        if (addResult.Succeeded)
                        {
                            finalresult.SuccessCount++;
                            if (UserInformation.ReplaceExisting && currentRoles.Any())
                            {
                                finalresult.Messages.Add($"User {user.UserName} was successfully removed from its old roles and was assigned to new ones.");
                            }
                      
                        }
                        else
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.AddRange(addResult.Errors.Select(e =>
                            $"{user.UserName} : {e.Description}"));
                            if (currentRoles.Any()&&UserInformation.ReplaceExisting)
                            {
                                var result = await _userManager.AddToRolesAsync(user, currentRoles);
                                if (result.Succeeded)
                                {
                                    finalresult.Messages.Add($"Something went wrong with assigning the new roles to the user {user.UserName} so we have just restored the old roles to the user again");
                                }
                                else
                                {
                                    finalresult.Messages.Add($"Something went wrong with assigning the new roles to the user {user.UserName}");
                                }
                               
                            }

                            
                        
                        }
                    }
                }

                // Commit if any successful updates
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
                _logger.LogError(ex, "Bulk role update failed");
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Messages.Clear();
                finalresult.Errors.Add("Failed to process bulk role updates. No effect happended to the database");
                return finalresult;
            }
        }
        //-------------------------------------------------------------------------------------------------------   
        public async Task<IEnumerable<ReturnUsersWithRolesAdminDto>> GetUsers()
        {
                return await _userManager.Users
                               .Include(u => u.UserRoles)
                               .ThenInclude(ur => ur.Role)
                               // Project to DTO using Mapster
                               .ProjectToType<ReturnUsersWithRolesAdminDto>()
                               .ToListAsync();
        }
        //-------------------------------------------------------------------------------------------------------   
        public async Task<FinalResult> DeleteAccounts(List<string> userIDs, string currentAdminId)
        {
            using var transaction = await _dbcontext.Database.BeginTransactionAsync();
            var finalresult = new FinalResult();
            try
            {
                var currentAdmin = await _userManager.FindByIdAsync(currentAdminId);
                if (currentAdmin == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("The requester is not existing in the database!");
                    return finalresult;
                }

                var distinctIds = userIDs.Distinct().ToList(); //The Distinct means that it will filter all the IDs that was came by the admin to be all unique (delete any dublicated id)
                var users = await _userManager.Users
                    .Where(u => distinctIds.Contains(u.Id))
                    .Include(u => u.UserRoles) 
                    .ToListAsync();

                var userMap = users.ToDictionary(u => u.Id); //This line converts the list of user objects into a dictionary, where the key is the user’s ID, and the value is the corresponding user object.  Benefit:  When you need to access a particular user's details during your deletion process, you can quickly look up the user by their ID using this dictionary. Lookups in a dictionary are very fast (O(1) complexity on average) compared to searching through a list.
                var processedIds = new HashSet<string>(); //You'll use this to keep track of which user IDs have already been processed during your deletion loop. Benefit:       It ensures that each user ID is handled only once.The HashSet is an efficient way to verify if a particular user has already been processed(to avoid duplicate work or unintended side effects). So it's just a place to store the ID inside so we can know if a user was already processed (contain the a hashed id of the user already = the user was processed)



                
               

                foreach (var userId in distinctIds)  // For each user ID in your list of unique (distinct) IDs:
                {
                    if (!userMap.TryGetValue(userId, out var user))         // Try to retrieve the user object corresponding to the userId from the previously built dictionary (userMap).  If TryGetValue returns false, it means the user was not found in the dictionary.
                    {
                    
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"User with id {userId} not found inside the database. No changes happened to it");
                       
                        continue;
                    }

                    if (processedIds.Contains(userId)) continue;    // Check if this userId has already been processed (to avoid duplicate deletion attempts). // If it has, skip to the next iteration.

                    processedIds.Add(userId); 

                    var roles = await _userManager.GetRolesAsync(user); //this will get all the roles from the user that is stored inside the Dictionary after we know that it exist + wasn't processed yet.
                    if (roles.Contains(AppRoles.Admin))
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"Cannot delete the admin {user.UserName}");
                        continue;

                    }

                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (deleteResult.Succeeded)
                    {
                        finalresult.SuccessCount++;
                        finalresult.Messages.Add($"The user {user.UserName} was successfully deleted from the database");
                    }
                    else
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"Failed to delete user {user.UserName} because {string.Join(", ", deleteResult.Errors.Select(e => e.Description),1,deleteResult.Errors.Count()-1)}"); //because we have a list of errors in here we used the Join which is a method that is used when there is a list, and what it does is  takes the list of error description strings and concatenates them into one string, with each description separated by a comma and a space.

                    }
                }

                if (finalresult.SuccessCount > 0) //if we atleast deleted something then save the Transaction by commiting it.
                {
                    await transaction.CommitAsync();
                    finalresult.Succeeded = true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                    finalresult.Messages.Clear();
                    finalresult.Messages.Add("There was no deletion operation happened. No changes happened to the database");
                }

               
                return finalresult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Bulk delete failed for {Count} users", userIDs.Count);
                finalresult.SuccessCount = 0;
                finalresult.Succeeded = false;
                finalresult.Messages.Clear();
                finalresult.Errors.Add($"Bulk operation failed: {ex.Message}");
                return finalresult;
            }
        }
    }
}
