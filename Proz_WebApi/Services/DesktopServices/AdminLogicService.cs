using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Transactions;
using EasyCaching.Core;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Services;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Proz_WebApi.Models.DesktopModels.Dto.Auth;
using Proz_WebApi.Models.DesktopModels.DTO;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using SimpleCryptography.Business.EncryptionServices;
using StackExchange.Redis;
using Zxcvbn;

namespace Proz_WebApi.Services.DesktopServices
{
    public class AdminLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly JWTOptions _jwtoption;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AuthService> _loggerr;
        private readonly IEasyCachingProviderFactory _easyCachingFactory;
        private readonly EmailNormalizer _emailNormalizer;
        private readonly DomainVerifier _domainVerifier;
        private readonly SesEmailSender _EmailSender;

        private readonly VerificationCodeService _managingTempData;
        private readonly AesEncryptionService _aesEncryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

       public AdminLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager,
            JWTOptions jwtoption, ApplicationDbContext_Desktop dbcontext, ILogger<AuthService> loggerr, IEasyCachingProviderFactory easyCachingFactory,
            EmailNormalizer emailNormalizer, DomainVerifier domainVerifier, VerificationCodeService managingTempData
            , SesEmailSender EmailSender, AesEncryptionService aesEncryptionService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtoption = jwtoption;
            _dbcontext = dbcontext;
            _loggerr = loggerr;
            _easyCachingFactory = easyCachingFactory;
            _emailNormalizer = emailNormalizer;
            _domainVerifier = domainVerifier;
            _EmailSender = EmailSender;
            _managingTempData = managingTempData;
            _aesEncryptionService = aesEncryptionService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<FinalResult> InitializeSystemAsyncStepOne(GettingStartedStageOneDTO gettingstartedDTO)
        {
            var finalresult = new FinalResult();
            //using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            //{
            

                // 1. Check if system was already initialized (admin already exists?)
                var adminRole = await _roleManager.FindByNameAsync(AppRoles_Desktop.Admin);

            if (adminRole == null)
            {
                finalresult.Errors.Add("We couldn't complete the process, please call the system manager");
                finalresult.Succeeded = false;
                return finalresult;
            }

            int adminCount = await _dbcontext.UserRoles
                .Where(r => r.RoleId == adminRole.Id)
                .CountAsync();

                if (adminCount > 0)
                {
                finalresult.Succeeded = false;
                finalresult.Errors.Add("System already initialized.");
                    return finalresult;
                }

          

            string normalizedName = _emailNormalizer.NormalizeName(gettingstartedDTO.AdminUsername);
                string? normalizedEmail = _emailNormalizer.NormalizeEmail(gettingstartedDTO.AdminEmail);

            if (normalizedEmail == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Please enter a valid email that contains the '@' in it.");
                    return finalresult;
                }


               //var user = await _userManager.FindByEmailAsync(normalizedEmail);
               //if (user != null)
               //{
               //    finalresult.Succeeded = false;
               //    finalresult.Errors.Add($"User with the email {normalizedEmail} is already stored in our databases!");
               //    return finalresult;
               //}

                if (await _managingTempData.IsInCooldownAsync(normalizedEmail))
                  {
                      finalresult.Succeeded = false;
                      finalresult.Errors.Add("Please wait a few seconds before trying to register again.");
                      return finalresult;
                  }
             
            bool domainexisting = await _domainVerifier.HasValidDomainAsync(normalizedEmail);
                if (!domainexisting)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The email's domain that you have entered is not able to receive emails");
                    finalresult.Messages.Add("Please enter a valid email's domain in order to register");
                    return finalresult;
                }
                string code;
                try
                {
                    code = await _managingTempData.GenerateAndStoreCodeAsync(normalizedEmail);
                }
                catch (Exception ex)
                {
                    finalresult.Succeeded = false;
                    finalresult.Messages.Add("something went wront while saving user's data");
                    finalresult.Errors.Add($"we got an error while saving the temp data of the user inside our server. {Environment.NewLine} Please try again in other time.");

                    return finalresult;
                }
                string encryptedPassword = _aesEncryptionService.Encrypt(gettingstartedDTO.AdminPassword);
                var userTempData = new UserRegisterationTemp
                {
                    Username = normalizedName,
                    Email = normalizedEmail,
                    Password = encryptedPassword
                };
                try
                {
                    await _managingTempData.StoreUserRegistrationDataTemp(userTempData, normalizedEmail);
                }
                catch (Exception ex)
                {
                    finalresult.Succeeded = false;
                    finalresult.Messages.Add("something went wront while saving user's data");
                    finalresult.Errors.Add($"we got an error while saving the temp data of the user inside our server. {Environment.NewLine} Please try again in other time.");
                    return finalresult;
                }


                if (!await _EmailSender.SendVerificationCodeAsync(normalizedEmail, code))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Something went wrong while trying to send the email. Please try again later");
                    return finalresult;
                }
                
                await _managingTempData.StartCooldownAsync(normalizedEmail);
                finalresult.Succeeded = true;
                finalresult.Messages.Add($"Verification code has been sent to the email {normalizedEmail}, waiting the user to verify");
                return finalresult;
            
        }




        //-------------------------------------------------------------------------------------------------------   

        public async Task<FinalResult> InitializeSystemAsyncStepTwo(GettingStartedStageTwoDTO gettingstartedDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                var finalresult = new FinalResult();


                try
                {
                  

                    string? normalizedEmail = _emailNormalizer.NormalizeEmail(gettingstartedDTO.AdminEmail);
                    if (normalizedEmail == null)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Please enter a valid email that contains the '@' in it.");
                        return finalresult;
                    }
                    bool isValid = await _managingTempData.ValidateCodeAsync(normalizedEmail, gettingstartedDTO.Code);
                    if (!isValid)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Invalid or expired verification code.");
                        return finalresult;
                    }
                    var cache = await _managingTempData.GetUserRegistrationDataTemp(normalizedEmail);
                    if (cache == null)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Registration data expired. Please try again..");
                        return finalresult;
                    }
                    string normalizeUserName = _emailNormalizer.NormalizeName(cache.Username); //we don't trust redis here so we are normalizing the username and not just taking the username from redis blindly (because an attacker that may use the server and may change some users's data, because if it did then strange data will enter the database for example username all upper case)
                    var user = new ExtendedIdentityUsersDesktop
                    {
                        UserName = normalizeUserName,
                        Email = normalizedEmail
                    };
                    string decryptedPassword = _aesEncryptionService.Decrypt(cache.Password);
                    var result = await _userManager.CreateAsync(user, decryptedPassword);  //UserManager and RoleManager automatically save changes when you call methods like CreateAsync or AddToRoleAsync. When you use _dbcontext directly(e.g., adding a refresh token), you must call SaveChangesAsync. This < must answer your question about why we don't put SaveChangesAsync in every time we interact with the database.

                    if (!result.Succeeded)
                    {
                        finalresult.Errors.AddRange(result.Errors.Select(e => e.Description));
                        finalresult.Succeeded = false;
                        return finalresult;
                    }
                    if (!await _roleManager.RoleExistsAsync(AppRoles_Desktop.Admin))
                    {

                        throw new RoleNotFoundException(AppRoles_Desktop.Admin, user.UserName); //the throw will send the message to the  catch (Exception ex) ex variable so you log the error and what happend exactly inside the try statement from the catch statement.
                    }
                    var roleResult = await _userManager.AddToRoleAsync(user, AppRoles_Desktop.Admin);
                    if (!roleResult.Succeeded)
                    {

                        finalresult.Succeeded = false;
                        finalresult.Errors.AddRange(roleResult.Errors.Select(e => e.Description));
                        return finalresult;
                    }
                    var employee = new Employees
                    {
                        IdentityUsers_FK = user.Id,
                    };

                    _dbcontext.EmployeesTable.Add(employee);

                    var systemRow = new GettingStartedTable
                    {
                        CompanyName = gettingstartedDTO.CompanyName,
                        CurrenyType = gettingstartedDTO.Currency,
                        Admin_FK = user.Id,
                    };
                    await _dbcontext.PersonalInformationTable.AddAsync(new Personal_Information
                    {
                        FullName = gettingstartedDTO.FullName,
                        Age=Convert.ToInt32( gettingstartedDTO.Age),
                        DateOfBirth=gettingstartedDTO.Date_Of_Birth,
                        Gender=gettingstartedDTO.Gender,
                        Nationality=gettingstartedDTO.Nationality,
                        LivingOnPrimaryPlace = gettingstartedDTO.Living_On_Primary_Place,
                        IdentityUser_FK = user.Id
                    });
                    _dbcontext.GettingStartedTable.Add(systemRow);
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                    finalresult.Succeeded = true;
                    return finalresult;

                }

                catch (RoleNotFoundException ex) //this catch will only be executed if the RoleNotFoundException was thrown only but not the rest of other exception, so we have to create another global exception like   catch (Exception ex)
                {


                    _loggerr.LogError(ex, "User registration failed at {ErrorTime} because the role {RoleName} wasn't exisit to be assigned to the user {UserName}", ex.ErrorOccurredAt, ex.RoleName, ex.UserName);
                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("Registration failed due to a system error.");
                    return finalresult; //we didn't send the exact error to the user because he doesn't need to know that we could assign a role to him because of the server's error so we just send to him "faild registeration" and we logged the exact error.

                }
                catch (Exception ex)
                {

                    finalresult.Succeeded = false;
                    finalresult.Errors.Clear();
                    finalresult.Errors.Add("Un error occurred. Failed to register the user.");
                    return finalresult;
                }
            }


        }

        //-------------------------------------------------------------------------------------------------------   

        public async Task<FinalResult> CheckingGettingStatusService()
        {
            var finalresult = new FinalResult();
            int count = await _dbcontext.GettingStartedTable.CountAsync();
            if (count==0)
            {
                finalresult.Succeeded = false;
                return finalresult;
            }
            else
            {
                finalresult.Succeeded = true;
                return finalresult; 
            }
        }

        //-------------------------------------------------------------------------------------------------------   
        public async Task<IEnumerable<ReturnUsersWithRolesAdminDto>> GetAllUsers(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);
            var Requester = await _userManager.FindByIdAsync(requesterID.ToString());


            var users = await _dbcontext.Users
                .Include(u => u.UserRolesNA)
                    .ThenInclude(ur => ur.RoleNA)
                .Include(u => u.PersonalInformationNA).Where(c=>c.Id!= Requester.Id)
                .ToListAsync();

            List<ReturnUsersWithRolesAdminDto> result = new List<ReturnUsersWithRolesAdminDto>();

            foreach (var user in users)
            {
                var dto = new ReturnUsersWithRolesAdminDto
                {
                    id = user.Id,
                    FullName = user.PersonalInformationNA?.FullName ?? "Unknown",
                    RoleName = user.UserRolesNA.Select(ur => ur.RoleNA.Name).FirstOrDefault() ?? "No Role Assigned",
                };
                string RoleName = user.UserRolesNA.Select(ur => ur.RoleNA.Name).FirstOrDefault() ?? "No Role Assigned";
                  switch (RoleName)
                {
                    case "User":
                        dto.IsUser = true;
                        break;
                    case "Employee":
                        dto.IsEmployee = true;
                        break;
                    case "Department Manager":
                        dto.IsDepartmentManager = true;
                        break;
                    case "HR Manager":
                        dto.IsHRManager = true;
                        break;
                    case "Admin":
                        dto.IsAdmin = true;
                        break;
                
                }
                result.Add(dto);
            };     

            return result;

        }
        //-------------------------------------------------------------------------------------------------------   




      public async Task<FinalResult> UpdateRoles(string requesterId, UserInformationClass request)
{
    var finalResult = new FinalResult();

    try
    {
        // Get the admin making the request
        var requester = await _userManager.FindByIdAsync(requesterId);
        if (requester == null)
        {
            finalResult.Succeeded = false;
            finalResult.Errors.Add("Requester is invalid.");
            return finalResult;
        }

        var requesterRoles = await _userManager.GetRolesAsync(requester);
        if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
        {
            finalResult.Succeeded = false;
            finalResult.Errors.Add("Only Admins are allowed to change roles.");
            return finalResult;
        }

        // Get target user
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            finalResult.Succeeded = false;
            finalResult.Errors.Add("Target user not found.");
            return finalResult;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Don't allow changing other Admins
        if (currentRoles.Contains(AppRoles_Desktop.Admin))
        {
            finalResult.Succeeded = false;
            finalResult.Errors.Add("You are not allowed to change the role of another Admin.");
            return finalResult;
        }

        // Check if the role exists
        var roleExists = await _roleManager.RoleExistsAsync(request.NewRoles);
        if (!roleExists)
        {
            finalResult.Succeeded = false;
            finalResult.Errors.Add($"The role '{request.NewRoles}' does not exist.");
            return finalResult;
        }

        using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
        {
            // Remove all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add($"Failed to remove current roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                return finalResult;
            }

            // Add the new role
            var addResult = await _userManager.AddToRoleAsync(user, request.NewRoles);
            if (!addResult.Succeeded)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add($"Failed to add new role: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                return finalResult;
            }

            scope.Complete();
        }

        finalResult.Succeeded = true;
        finalResult.Messages.Add($"User {user.UserName} was successfully assigned to role '{request.NewRoles}'");
        return finalResult;
    }
    catch (Exception ex)
    {
        _loggerr.LogError(ex, "Error occurred during role update.");
        finalResult.Succeeded = false;
        finalResult.Errors.Add("An error occurred while updating roles. No changes were made.");
        return finalResult;
    }
}






        public async Task<FinalResult> DeleteAccount(Guid userID, string currentAdminId)
        {
            var finalresult = new FinalResult();

            try
            {
                // Step 1: Validate the admin making the request
                var currentAdmin = await _userManager.FindByIdAsync(currentAdminId);
                if (currentAdmin == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The requester does not exist in the database!");
                    return finalresult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(currentAdmin);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Only Admins are allowed to delete accounts.");
                    return finalresult;
                }

                // Step 2: Get the target user
                var userToDelete = await _userManager.FindByIdAsync(userID.ToString());
                if (userToDelete == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The user you are trying to delete does not exist.");
                    return finalresult;
                }

                // Step 3: Prevent deletion of another Admin
                var userRoles = await _userManager.GetRolesAsync(userToDelete);
                if (userRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("You are not allowed to delete another Admin.");
                    return finalresult;
                }
                string fullname= userToDelete.PersonalInformationNA?.FullName ?? "Unknown User";

                // Step 4: Delete the user
                var deleteResult = await _userManager.DeleteAsync(userToDelete);
                if (deleteResult.Succeeded)
                {
                    finalresult.Succeeded = true;
                    finalresult.Messages.Add($"User {fullname} was successfully deleted.");
                }
                else
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add($"Failed to delete user: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                }

                return finalresult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "An error occurred while deleting user account.");

                finalresult.Succeeded = false;
                finalresult.Errors.Add("An unexpected error occurred while trying to delete the account.");
                return finalresult;
            }
        }





    }
}
