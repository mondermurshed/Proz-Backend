using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.DTO.HRManager;
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

        public async Task<FinalResultBulkOperations> UpdateRoles(string requesterId, RoleUpdateDTO request) //---------------------------------------------
        {

            var finalresult = new FinalResultBulkOperations();

            try
            {

                var requester = await _userManager.FindByIdAsync(requesterId.ToString());
                if (requester == null)
                {

                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The requester is unknown. Couldn't done the operation");
                    return finalresult;
                }
                var requesterRoles = await _userManager.GetRolesAsync(requester);
                bool requesterIsHR = requesterRoles.Contains(AppRoles_Desktop.HRManager) && !requesterRoles.Contains(AppRoles_Desktop.Admin);

                // Get all unique user IDs and roles from request
                var userIds = request.UserInformation.Select(rc => rc.UserId).Distinct().ToList(); //Think about it like when we put a select method then this mean "please focus in this property and ignore anything else (ignore other properties)" request.UserInformation we say here enter this object (which is a list) and first go to the first element (which is an object) then select only someting (property) called UserId and ignore any other property in this object. After you are focusing this property Distinct() meaning take only the unique ones (do not dublicate values). After checking the first object it will go to the second object and check its ID property "is it the same as the first object's id ? if yes delete one of them, if not then contunue to the third object, does this id value has something similar to the ones on the first and second objects ? if yes delete all of them and keep one (logically it will keep the last checking object and delete the previous ones). after moving from one object to other and delete those who share the same id value as previous object, we will got at the end all unique object that has different id values. Do you remember that we said when we select something then we as saying it "select this thing and ignore anything else" then we are not just ignoring anything else but also we are meaning here return only IDs (BASED ON OUR EXAMPLE). You can see that userIds is a list of strings not a list of RoleChangeRequestAdmin (objects). So userIds is a list of unique IDs and noting more. 
                var allRoles = request.UserInformation
                    .SelectMany(rc => rc.NewRoles) //for example here is a story if you got it you understand both the select and selectmany. You are a teacher, and you have a list of 3 students. Each student has: an ID card (UserId), a list of Roles (like "Admin", "User", etc.) and a small note that says whether to replace roles or not (ReplaceExisting).SELECT – “Give me just one thing from each student” Now imagine you're saying this: “Go to each student and give me just their ID card (UserId), I don’t care about anything else.” so this method will return a list of string (list of IDs), remember!! Select() means: "Give me something from each object and return it in the same shape." even if that thing is a list! because the select method is commanded to take one thing from each user and if that one thing is a list then it will still take it! so if we select a list of roles inside each student then this method will not return a list but a list of lists because it will just do the job and take the one thing which is a list and we have three students so three lists. but what if we used a selectmany ? selectmany works as the following "Give me all the roles from all students, and put them in one big box" not inside seperate small box for each user like the select method does but combine all into a single box, in otherwords It goes inside each student’s role list, takes all the roles out, and puts them into one single big list., so selectmany here will return just a list of string not a list of lists. This is why allRoles is a list<string> only. So these are the scenarios that i can think of 
                    .Distinct() //so the final result here we will get a list of string that has all the needed roles. for example let's say user1 has {admin,user}, user2 has {admin,departmentmanager}, user3 has {user,admin,user} then the result will be a big string box (list[string]) that will be = admin,user,admin,departmentmanager,user,admin,user    and then they will become only unique roles like = admin,user,departmentmanager. This is how selectmany works.
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
                    finalresult.Succeeded = false;
                    finalresult.Messages.Add("Try to select valid users and roles then try again. No changes was happened to the database");
                    return finalresult;
                }
                var dedupedUserInfo = request.UserInformation
               .GroupBy(ui => ui.UserId)
               .Select(g => g.First())
               .ToList();

                // Process each user's role changes
                foreach (var UserInformation in dedupedUserInfo)
                {
                    var user = users.First(u => u.Id == UserInformation.UserId);
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    

                      bool assigningOrRemovingUserRole =
                      UserInformation.NewRoles.Contains(AppRoles_Desktop.User) ||
                      currentRoles.Contains(AppRoles_Desktop.User);

                    if (assigningOrRemovingUserRole)
                    {
                        finalresult.FailedCount++;
                        finalresult.Errors.Add($"You are not allowed to assign or remove the '{AppRoles_Desktop.User}' role using this service.");
                        continue;
                    }

                    if (requesterIsHR)
                    {

                        if (user == null)
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"The user with ID {UserInformation.UserId} does not exist.");
                            continue;
                        }



                        // ❌ Can't modify if target is already an HR Manager or Admin
                        if (currentRoles.Contains(AppRoles_Desktop.HRManager) || currentRoles.Contains(AppRoles_Desktop.Admin))
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"You are not allowed to modify the roles of user {user.UserName} because they are a high-privileged user.");
                            continue;
                        }

                        // ❌ Can't assign HR Manager or Admin roles
                        if (UserInformation.NewRoles.Contains(AppRoles_Desktop.HRManager) || UserInformation.NewRoles.Contains(AppRoles_Desktop.Admin))
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"You are not allowed to assign HR Manager or Admin role to the user {user.UserName}.");
                            continue;
                        }
                    }

                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        List<string> rolesToAdd = new List<string>();
                        // Remove existing roles if replacing
                        if (currentRoles.Contains(AppRoles_Desktop.Admin))
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"We can't assign anything to the user {user.UserName} because he got the {AppRoles_Desktop.Admin} role");
                            continue;
                        }

                        if (UserInformation.ReplaceExisting && currentRoles.Any())
                        {
                            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            if (!removeResult.Succeeded)
                            {
                                finalresult.FailedCount++;
                                finalresult.Errors.Add($"Something went wrong for the user {user.UserName} because {string.Join(", ", removeResult.Errors.Select(e => e.Description), 1, removeResult.Errors.Count() - 1)} No operation was done for this user.");
                                continue;
                            }
                            rolesToAdd = UserInformation.NewRoles.ToList();

                        }
                        else
                        {
                            foreach (var curRole in currentRoles)
                            {
                                if (UserInformation.NewRoles.Contains(curRole))
                                {
                                    finalresult.Errors.Add($"User {user.UserName} already have the role {curRole}. Nothing will be done here.");
                                }
                            }
                            rolesToAdd = UserInformation.NewRoles.Except(currentRoles).ToList();
                        }





                        if (rolesToAdd.Any())
                        {

                            //bool AddedUserRole = rolesToAdd.Contains(AppRoles_Desktop.User);
                            //bool AddedEmployeeRole = rolesToAdd.Contains(AppRoles_Desktop.Employee);
                            //bool ContainUserRole = currentRoles.Contains(AppRoles_Desktop.User);
                            //bool ContainEmployee = currentRoles.Contains(AppRoles_Desktop.Employee);

                            //bool Case1 = AddedUserRole && AddedEmployeeRole;
                            //bool Case2 = ContainUserRole || ContainEmployee;
                            //bool Case3 = (AddedUserRole || AddedEmployeeRole) && (ContainUserRole || ContainEmployee);
                            //bool Case4 = (AddedUserRole || AddedEmployeeRole) && rolesToAdd.Count > 1;
                            //if (!UserInformation.ReplaceExisting && (Case1 || Case2 || Case3))
                            //{

                            //    finalresult.Errors.Add($"the user {user.UserName} can only have a user role or an employee role and not the both together.");
                            //    finalresult.FailedCount++;

                            //    continue;
                            //}
                            //else if (UserInformation.ReplaceExisting && (Case1 || Case4))
                            //{
                            //    //restoringUserData.Rollback();
                            //    finalresult.Errors.Add($"the user {user.UserName} can only have {AppRoles_Desktop.User} role or {AppRoles_Desktop.Employee} role and not the both together.");
                            //    finalresult.FailedCount++;

                            //    continue;
                            //}
                            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                            if (addResult.Succeeded)
                            {
                                finalresult.SuccessCount++;
                                if (UserInformation.ReplaceExisting && currentRoles.Any())
                                {
                                    scope.Complete();
                                    finalresult.Messages.Add($"User {user.UserName} was successfully removed from its old roles and was assigned to new ones.");
                                    continue;
                                }
                                else
                                {
                                    scope.Complete();
                                    finalresult.Messages.Add($"User {user.UserName} was successfully assigned to the new roles");
                                    continue;
                                }

                            }
                            else
                            {
                                finalresult.FailedCount++;
                                finalresult.Errors.Add($"Something went wrong for the user {user.UserName} because {string.Join(", ", addResult.Errors.Select(e => e.Description), 1, addResult.Errors.Count() - 1)} No operation was done for this user.");
                                continue;
                            }

                        }
                        else
                        {
                            //restoringUserData.Rollback();
                            finalresult.SkippedCount++;
                            finalresult.Errors.Add($"Nothing was performed for the user {user.UserName} because you didn't assign any roles or it was having these roles already");
                            continue;
                        }
                    }
                }


                if (finalresult.SuccessCount > 0)
                {
                    //await transaction.CommitAsync();
                    finalresult.Succeeded = true;
                }
                else
                {
                    //await transaction.RollbackAsync();
                    finalresult.Succeeded = false;
                }

                return finalresult;
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                _logger.LogError(ex, "Bulk role update failed");
                finalresult.Succeeded = false;
                finalresult.Errors.Clear();
                finalresult.Messages.Clear();
                finalresult.Errors.Add("Failed to process bulk role updates. No effect happended to the database");
                return finalresult;
            }
        }


        public async Task<FinalResultBulkOperations> DeleteAccounts(List<Guid> userIDs, string currentAdminId) //--------------------------------------------
        {

            var finalresult = new FinalResultBulkOperations();
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
                var requesterRoles = await _userManager.GetRolesAsync(currentAdmin);
                bool requesterIsHR = requesterRoles.Contains(AppRoles_Desktop.HRManager) && !requesterRoles.Contains(AppRoles_Desktop.Admin);

                var distinctIds = userIDs.Distinct().ToList(); //The Distinct means that it will filter all the IDs that was came by the admin to be all unique (delete any dublicated id)
                var users = await _userManager.Users
                    .Where(u => distinctIds.Contains(u.Id))
                    .Include(u => u.UserRolesNA)
                    .ToListAsync();

                var userMap = users.ToDictionary(u => u.Id); //This line converts the list of user objects into a dictionary, where the key is the user’s ID, and the value is the corresponding user object.  Benefit:  When you need to access a particular user's details during your deletion process, you can quickly look up the user by their ID using this dictionary. Lookups in a dictionary are very fast (O(1) complexity on average) compared to searching through a list.
                var completed = new HashSet<Guid>(); //You'll use this to keep track of which user IDs have already been processed during your deletion loop. Benefit:       It ensures that each user ID is handled only once.The HashSet is an efficient way to verify if a particular user has already been processed(to avoid duplicate work or unintended side effects). So it's just a place to store the ID inside so we can know if a user was already processed (contain the a hashed id of the user already = the user was processed)






                foreach (var userId in distinctIds)  // For each user ID in your list of unique (distinct) IDs:
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (!userMap.TryGetValue(userId, out var user))         // Try to retrieve the user object corresponding to the userId from the previously built dictionary (userMap).  If TryGetValue returns false, it means the user was not found in the dictionary.
                        {

                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"User with id {userId} not found inside the database. Nothing was done here.");

                            continue;
                        }

                        if (completed.Contains(userId)) continue;    // Check if this userId has already been processed (to avoid duplicate deletion attempts). // If it has, skip to the next iteration.

                        completed.Add(userId);

                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains(AppRoles_Desktop.Admin))
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"Cannot delete the admin {user.UserName}");
                            continue;
                        }


                        if (requesterIsHR)
                        {
                            bool isAllowed = roles.All(r =>
                                r == AppRoles_Desktop.User || r == AppRoles_Desktop.Employee);

                            if (!isAllowed)
                            {
                                finalresult.FailedCount++;
                                finalresult.Errors.Add($"You are not allowed to delete {user.UserName} because their role is higher than allowed.");
                                continue;
                            }
                        }


                        var deleteResult = await _userManager.DeleteAsync(user);
                        if (deleteResult.Succeeded)
                        {
                            scope.Complete();
                            finalresult.SuccessCount++;
                            finalresult.Messages.Add($"The user {user.UserName} was successfully deleted from the database");
                        }
                        else
                        {
                            finalresult.FailedCount++;
                            finalresult.Errors.Add($"Failed to delete user {user.UserName} because {string.Join(", ", deleteResult.Errors.Select(e => e.Description), 1, deleteResult.Errors.Count() - 1)}"); //because we have a list of errors in here we used the Join which is a method that is used when there is a list, and what it does is  takes the list of error description strings and concatenates them into one string, with each description separated by a comma and a space.

                        }
                    }
                }

                if (finalresult.SuccessCount > 0) //if we atleast deleted something then save the Transaction by commiting it.
                {

                    finalresult.Succeeded = true;
                }
                else
                {

                    finalresult.Succeeded = false;

                }


                return finalresult;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Bulk delete failed for {Count} users", userIDs.Count);
                finalresult.Succeeded = false;
                finalresult.Errors.Add($"Bulk operation failed: {ex.Message}");
                return finalresult;
            }
        }


    }
}
