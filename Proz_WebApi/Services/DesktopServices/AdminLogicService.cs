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
using EFCore.BulkExtensions;
using Polly;
using Proz_WebApi.Models.DesktopModels.DTO.Auth;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.SignalR;
using Proz_WebApi.Helpers_Services.SignleR_Logic;


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
        private readonly IHubContext<MainHub> _hub;
        public AdminLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager,
            JWTOptions jwtoption, ApplicationDbContext_Desktop dbcontext, ILogger<AuthService> loggerr, IEasyCachingProviderFactory easyCachingFactory,
            EmailNormalizer emailNormalizer, DomainVerifier domainVerifier, VerificationCodeService managingTempData
            , SesEmailSender EmailSender, AesEncryptionService aesEncryptionService, IHttpContextAccessor httpContextAccessor
           , IHubContext<MainHub> hub)
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
            _hub = hub;
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
                var userTempData = new GettingStartedStageOneTemp
                {
                    AdminUsername = normalizedName,
                    AdminEmail = normalizedEmail,
                    AdminPassword = encryptedPassword,
                    Age=gettingstartedDTO.Age,
                    Date_Of_Birth=gettingstartedDTO.Date_Of_Birth,
                    Gender=gettingstartedDTO.Gender,
                    PaymentFrequency = gettingstartedDTO.PaymentFrequency,
                    Nationality =gettingstartedDTO.Nationality,
                    Living_On_Primary_Place=gettingstartedDTO.Living_On_Primary_Place,
                    CompanyName=gettingstartedDTO.CompanyName,
                    Currency=gettingstartedDTO.Currency,
                    FullName=gettingstartedDTO.FullName
                   
                };
                try
                {
                    await _managingTempData.StoreAdminRegistrationDataTemp(userTempData, normalizedEmail);
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
                    var cache = await _managingTempData.GetAdminRegistrationDataTemp(normalizedEmail);
                    if (cache == null)
                    {
                        finalresult.Succeeded = false;
                        finalresult.Errors.Add("Registration data expired. Please try again..");
                        return finalresult;
                    }
                    string normalizeUserName = _emailNormalizer.NormalizeName(cache.AdminUsername); //we don't trust redis here so we are normalizing the username and not just taking the username from redis blindly (because an attacker that may use the server and may change some users's data, because if it did then strange data will enter the database for example username all upper case)
                    var user = new ExtendedIdentityUsersDesktop
                    {
                        UserName = normalizeUserName,
                        Email = normalizedEmail
                    };
                    string decryptedPassword = _aesEncryptionService.Decrypt(cache.AdminPassword);
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

                   await _dbcontext.EmployeesTable.AddAsync(employee);

                    var systemRow = new GettingStartedTable
                    {
                        CompanyName = cache.CompanyName,
                        CurrenyType = cache.Currency,
                        PaymentFrquency = cache.PaymentFrequency,
                        Admin_FK = user.Id,
                        
                    };
                  await  _dbcontext.GettingStartedTable.AddAsync(systemRow);
                    await _dbcontext.PersonalInformationTable.AddAsync(new Personal_Information
                    {
                        FullName = cache.FullName,
                        Age=Convert.ToInt32( cache.Age),
                        DateOfBirth=cache.Date_Of_Birth,
                        Gender=cache.Gender,
                        Nationality=cache.Nationality,
                        LivingOnPrimaryPlace = cache.Living_On_Primary_Place,
                        
                        IdentityUser_FK = user.Id
                    });
                 
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





        public async Task<FinalResult> InitializeSystemAsyncResendData(ResendVerificationCodeDTO resendcode)  //--------------------------------------------------
        {
            var finalResult = new FinalResult();
            string? normalizedEmail = _emailNormalizer.NormalizeEmail(resendcode.Email);

            if (normalizedEmail == null)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Please enter a valid email.");
                return finalResult;
            }



            try
            {
                if (await _managingTempData.IsInCooldownAsync(normalizedEmail))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Please wait few seconds from the moment you sent the previous code!");
                    return finalResult;
                }
                var existingData = await _managingTempData.GetAdminRegistrationDataTemp(normalizedEmail);
                if (existingData == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("No pending registration found for this email.");
                    return finalResult;
                }
                string newCode = await _managingTempData.GenerateAndStoreCodeAsync(normalizedEmail);
                await _managingTempData.StoreAdminRegistrationDataTemp(existingData, normalizedEmail);
                if (!await _EmailSender.SendVerificationCodeAsync(normalizedEmail, newCode))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Something went wrong while trying to send the email. Please try again later");
                    return finalResult;
                }
                await _managingTempData.StartCooldownAsync(normalizedEmail);
                finalResult.Succeeded = true;
                finalResult.Messages.Add("Verification code has been resent to your email.");

            }
            catch (Exception ex)
            {
                finalResult.Succeeded = false;
                finalResult.Errors.Add("An error occurred. Please try again later.");
            }

            return finalResult;
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

        Guid requesterID;
        try
        {
            requesterID = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
        }
       
        catch
        {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
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

             var employeerow = _dbcontext.EmployeesTable.Where(a => a.IdentityUsers_FK == user.Id).FirstOrDefault();
             if (employeerow == null)
            {
            var employee = new Employees
            {

                IdentityUsers_FK = user.Id,

            };

            await _dbcontext.EmployeesTable.AddAsync(employee);
            await _dbcontext.SaveChangesAsync();
            employeerow = _dbcontext.EmployeesTable.Where(a => a.IdentityUsers_FK == user.Id).FirstOrDefault();
                    }



             var employee_departmentrow = _dbcontext.EmployeeDepartmentsTable.Where(a => a.Employee_FK == employeerow!.Id).FirstOrDefault();
            if (employee_departmentrow == null && request.NewRoles!=AppRoles_Desktop.Admin)
            {
            var employeedepartment = new Employee_Departments
            {
                Employee_FK = employeerow.Id,

              
            };

            await _dbcontext.EmployeeDepartmentsTable.AddAsync(employeedepartment);
            await _dbcontext.SaveChangesAsync();
           }
            if(request.NewRoles == AppRoles_Desktop.Admin)
            {
              foreach(var record in _dbcontext.EmployeeDepartmentsTable.Where(e => e.Employee_FK == employeerow.Id).ToList())
              {
                  _dbcontext.EmployeeDepartmentsTable.Remove(record);
              }
                           
            }

               var countmanagertowhat = await _dbcontext.DepartmentsTable.Where(e => e.Manager_FK == user.Id).CountAsync();
               if (request.NewRoles == AppRoles_Desktop.HRManager && countmanagertowhat>1)
               {
                   finalResult.Succeeded = false;
                   finalResult.Errors.Add($"An HR Manager can only be manager at one department, please unassign it from all other extra departments");
                   return finalResult;
               }
           
               if ((request.NewRoles == AppRoles_Desktop.User || request.NewRoles == AppRoles_Desktop.Employee || request.NewRoles == AppRoles_Desktop.Admin) && await _dbcontext.DepartmentsTable.Where(e => e.Manager_FK == user.Id).AnyAsync())
               {
                   finalResult.Succeeded = false;
                   finalResult.Errors.Add($"A person with the {request.NewRoles} role can't be a manager to any department in the system");
                   return finalResult;
               }


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
                    string? targetName =await _dbcontext.Users.Where(u => u.Id == user.Id).Select(u => u.PersonalInformationNA.FullName).FirstOrDefaultAsync();
                    if (targetName == null)
                        targetName = "**No Name**";
                    var log = new Audit_Logs
                    {
                        ActionType = "Updating",
                        Notes = $"Update user role of the user '{targetName}' to '{request.NewRoles}'",
                        PerformerAccount_FK = requesterID,
                        TargetEntity_FK = employeerow.Id
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
        }

                string user_fullname = await _dbcontext.Users.Include(u => u.PersonalInformationNA).Where(a => a.Id == request.UserId).Select(u => u.PersonalInformationNA.FullName).FirstOrDefaultAsync() ?? "Unknown User";
                finalResult.Succeeded = true;
                finalResult.Messages.Add($"User {user_fullname} was successfully assigned to role '{request.NewRoles}'");
                // Corrected code for the RoleChangedEvent instantiation
                var roleChangedEvent = new RoleChangedEvent
                {
                    RoleName = request.NewRoles
                };
                     await _hub.Clients.User(user.Id.ToString()).SendAsync("RoleChanged", roleChangedEvent);
                return finalResult;
    }
  catch (Exception ex)
    {
        _loggerr.LogError(ex, "Error occurred during role update.");
        finalResult.Succeeded = false;
        finalResult.Errors.Add($"An error occurred while updating roles. No changes were made. {ex}");
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
                Guid requesterID;
                try
                {
                     requesterID = await _dbcontext.Users.Where(c => c.Id == currentAdmin.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }
                
                catch
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Invalid requester");
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

                Guid TargetID;
                try
                {
                     TargetID = await _dbcontext.Users.Where(c => c.Id == userToDelete.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }
                 catch
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Invalid Target");
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


                //a
                string user_fullname =await _dbcontext.Users.Include(u => u.PersonalInformationNA).Where(a => a.Id == userID).Select(u => u.PersonalInformationNA.FullName).FirstOrDefaultAsync() ?? "Unknown User";

                // Step 4: Delete the user
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
;
                    var auditLogs = await _dbcontext.AuditLogsTable
                    .Where(log => log.PerformerAccount_FK == TargetID)
                    .ToListAsync();

                    // Step 2: Modify them in memory
                    foreach (var log1 in auditLogs)
                    {
                        log1.PerformerAccount_FK = null; // or set to default system user ID
                    }

                    await _dbcontext.BulkUpdateAsync(auditLogs);


                    var deleteResult = await _userManager.DeleteAsync(userToDelete);
             
                    if (!deleteResult.Succeeded)
                    {

                        finalresult.Succeeded = false;
                        finalresult.Errors.Add($"Failed to delete the user: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                        return finalresult;
                    }
                    var log = new Audit_Logs
                    {
                        ActionType = "Deleting",
                        Notes = $"Deletes Account Of The User '{user_fullname}'",
                        PerformerAccount_FK = requesterID,
                        TargetEntity_FK = TargetID
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);

                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
                finalresult.Succeeded = true;
                finalresult.Messages.Add($"User {user_fullname} was successfully deleted.");
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



        public async Task<FinalResult> CreateANewfeedbackType(string requesterId, CreateAFeedbackTypeRequest request)
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
               


                Guid requesterID;
                try
                {
                  requesterID = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to Insert Feedback types.");
                    return finalResult;
                }
                 int count= _dbcontext.FeedbacksTypesTable.Where(ft => ft.FeedbackType == request.feedbackTypeName).Count();
                if(count != 0)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Feedback type {request.feedbackTypeName} already exists.");
                    return finalResult;
                }

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                   
                await _dbcontext.FeedbacksTypesTable.AddAsync(new Feedback_Types
                {
                    FeedbackType = request.feedbackTypeName,
                });
                    var log = new Audit_Logs
                    {
                        ActionType = "Creating",
                        Notes = $"Creates Feedback Type '{request.feedbackTypeName}'",
                        PerformerAccount_FK = requesterID,
                        TargetEntity_FK = null
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);

                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }

             
                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Feedback {request.feedbackTypeName} was successfully inserted as a feedback type in the system.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during role update.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add("Couldn't save the new feedback type, please wait and try again later.");
                return finalResult;
            }
        }

        public async Task<IEnumerable<ReturnAllFeedbackTypes>> GetFeedbackTypes(string requesterId)
        {
        

            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {
                    
                    return null;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                   
                    return null;
                }


              
                var result = await _dbcontext.FeedbacksTypesTable
                .Select(x => new ReturnAllFeedbackTypes
                {
                    Id = x.id,
                    FeedbackTypeName = x.FeedbackType
                }).ToListAsync();


                return result;




            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during fetching feedback types by an admin.");
                return null;
            }
        }



        public async Task<FinalResult> RemoveAFeedbackType(RemoveFeedbackTypeDTO request, string currentAdminId)
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
              
                Guid requesteridmanager;
                try
                {
                     requesteridmanager = await _dbcontext.Users.Where(c => c.Id == currentAdmin.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Invalid requester");
                    return finalresult;
                }
                var requesterRoles = await _userManager.GetRolesAsync(currentAdmin);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Only Admins are allowed to delete accounts.");
                    return finalresult;
                }

                var feedbacktype = await _dbcontext.FeedbacksTypesTable.Where(ft => ft.id == request.FeedbackID).FirstOrDefaultAsync();
                if (feedbacktype == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Couldn't find the feedback type");
                    return finalresult;
                }

           

                var count =await  _dbcontext.FeedbacksTable.Where(ft => ft.FeedbackType_FK == request.FeedbackID).CountAsync();

                if (count != 0 && string.IsNullOrWhiteSpace(request.ReplaceWith))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The current feedback you wanted to delete is being used by other feedbacks records. \n we can't delete it but you can still replace it with something that has the similar meaning. \n Please insert in the textbox the thing you want to replace it with.");
                    return finalresult;
                }
                else if (count==0)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        _dbcontext.FeedbacksTypesTable.Remove(feedbacktype);

                        var log = new Audit_Logs
                        {
                            ActionType = "Deleting",
                            Notes = $"Deletes Feedback Type '{feedbacktype.FeedbackType}'",
                            PerformerAccount_FK = requesteridmanager,
                            TargetEntity_FK = null
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);


                        await _dbcontext.SaveChangesAsync();
                        finalresult.Succeeded = true;
                        finalresult.Messages.Add($"Feedback Type was successfully deleted.");


                        scope.Complete();
                        return finalresult;

                    

                    }
                }
                else
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        feedbacktype.FeedbackType = request.ReplaceWith;

                        var log = new Audit_Logs
                        {
                            ActionType = "Updating",
                            Notes = $"Replaces Feedback Type name from '{feedbacktype.FeedbackType}' to '{request.ReplaceWith}'",
                            PerformerAccount_FK = requesteridmanager,
                            TargetEntity_FK = null
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                        await _dbcontext.SaveChangesAsync();
                        scope.Complete();
                    finalresult.Succeeded = true;
                    finalresult.Messages.Add($"Feedback Type was successfully replaced with {request.ReplaceWith}.");
                    return finalresult;
                    }
                }
              
                // Step 4: Delete the user
              
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, $"An error occurred while deleting the feedback with the GUID {request.FeedbackID}");

                finalresult.Succeeded = false;
                finalresult.Errors.Add("Couldn't delete the feedback type.");
                return finalresult;
            }
        }



        public async Task<IEnumerable<ReturnAllDepartmentManagers>> GetAllDepartmentManagers(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);
            var Requester = await _userManager.FindByIdAsync(requesterID.ToString());


            var departmentManagers = await _dbcontext.UserRoles
        .Where(ur => ur.RoleNA.Name == AppRoles_Desktop.DepartmentManager)
        .Select(ur => new ReturnAllDepartmentManagers
        {
            UserId = ur.UserNA.EmployeesNA.Id,
            FullName = ur.UserNA.PersonalInformationNA.FullName
        })
        .ToListAsync();



            return departmentManagers;

        }


        public async Task<FinalResult> CreateANewDepartment(string requesterId, DepartmentCreatingRequest request)
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


           

                Guid requesteridmanager;
                try
                {
                     requesteridmanager = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to create a new department in the system.");
                    return finalResult;
                }
                bool existingname =await _dbcontext.DepartmentsTable.Where(d=>d.DepartmentName==request.DepartmentName).AnyAsync();
                if(existingname==true)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"There is already a department with the name '{request.DepartmentName}' inside the system.");
                    return finalResult;
                }
                var employee = await _dbcontext.EmployeesTable
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.ManagerID);

                if (employee == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Department manager doesn't located.");
                    return finalResult;
                }
                var hasRole = await _dbcontext.EmployeesTable
                .Where(e => e.Id == employee.Id)
                .Select(e => e.IdentityUserNA)          // navigate to the user
                .SelectMany(u => u.UserRolesNA)         // get user's roles
                .AnyAsync(ur => ur.RoleNA.Name == AppRoles_Desktop.DepartmentManager);

                if(hasRole==false)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"The user you are trying to assign as a manager to a department doee not has the {AppRoles_Desktop.DepartmentManager} role.");
                    return finalResult;
                }
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var departmentobject = new Departments
                    {
                        DepartmentName = request.DepartmentName,
                        Manager_FK = request.ManagerID
                    };
                    await _dbcontext.AddAsync(departmentobject);
                    var log = new Audit_Logs
                    {
                        ActionType = "Creating",
                        Notes = $"Creates department '{request.DepartmentName}'",
                        PerformerAccount_FK = requesteridmanager,
                        TargetEntity_FK = employee.Id
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
                var departmentmanagerfullname = await _dbcontext.EmployeesTable.AsNoTracking().Where(e => e.Id == employee.Id)
                    .Select(e => e.IdentityUserNA.PersonalInformationNA.FullName)
                    .FirstOrDefaultAsync();


                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Department with the name of {request.DepartmentName} was successfully created as a department in the system and the user {departmentmanagerfullname} as a manager to it.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during role update.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add("We couldn't create the new department.");
                return finalResult;
            }
        }


       public async Task<IEnumerable<ReturnDepartmentsWithManagers>> GetAllDepartmentsAlongWithItsManagers(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);
           
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var departmentManagers = await _dbcontext.DepartmentsTable
           .Select(ur => new ReturnDepartmentsWithManagers
           {
             DepartmentName=ur.DepartmentName,
               ManagerName = ur.ManagerNA != null && ur.ManagerNA.IdentityUserNA != null &&
                  ur.ManagerNA.IdentityUserNA.PersonalInformationNA != null
                  ? ur.ManagerNA.IdentityUserNA.PersonalInformationNA.FullName
                  : "**Not Assigned To Any Manager**",
               DepartmentID = ur.Id,
               ManagerID = ur.ManagerNA != null ? ur.ManagerNA.Id : Guid.Empty
           })
           .ToListAsync();



            return departmentManagers;

        } 



        public async Task<FinalResult> UnassignAManagerfromADepartment(string requesterId, UnassignAManagerFromADepartmentRequest request)
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
              
                Guid requesteridmanager;
                try
                {
                     requesteridmanager = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to create a new department in the system.");
                    return finalResult;
                }

                var department = await _dbcontext.DepartmentsTable.Where(c=>c.Id==request.DepartmentID).FirstOrDefaultAsync();
                if(department == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department was not found in the system.");
                    return finalResult;
                }
                if(department.Manager_FK==null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department {department.DepartmentName} is already not having a manager assigned to it.");
                    return finalResult;
                }
                var manager = await _dbcontext.EmployeesTable.Where(e=>e.Id==department.Manager_FK).FirstOrDefaultAsync();
                if(manager==null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Data was not located about the manager.");
                    return finalResult;
                }
                string managername = await _dbcontext.EmployeesTable.AsNoTracking()
               .Where(e => e.Id == department.Manager_FK)
              .Select(e => e.IdentityUserNA.PersonalInformationNA.FullName)
              .FirstOrDefaultAsync() ?? "Unknown Manager";
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                   department.Manager_FK = null; // Unassign the manager by setting the foreign key to null

                    var log = new Audit_Logs
                    {
                        ActionType = "Updating",
                        Notes = $"Unassigning the department manager '{managername} from '{department.DepartmentName}'",
                        PerformerAccount_FK = requesteridmanager,
                        TargetEntity_FK = manager.Id
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);

                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
               


                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Department manager {managername} was successfully unassigned from the department {department.DepartmentName}.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during Unassigning a manager from a department.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add("We couldn't unassign the manager from the department.");
                return finalResult;
            }
        }


        public async Task<FinalResultDeleteDepartment> RemoveADepartment(RemoveADepartmentRequest request, string currentAdminId)
        {
            var finalresult = new FinalResultDeleteDepartment();

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
              
                Guid currentAdminID;
                try
                {
                    currentAdminID = await _dbcontext.Users.Where(c => c.Id == currentAdmin.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Invalid requester");
                    return finalresult;
                }
                var requestRoles = await _userManager.GetRolesAsync(currentAdmin);
                if(!requestRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Only Admins are allowed to delete/remove a department from the system.");
                    return finalresult;
                }
                var department = await _dbcontext.DepartmentsTable.Where(ft => ft.Id == request.DepartmentID).FirstOrDefaultAsync();
                if (department == null)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("Couldn't find the department in the system");
                    return finalresult;
                }

                var count = await _dbcontext.EmployeeDepartmentsTable.Where(ft => ft.Department_FK == department.Id).CountAsync();

                if (count != 0 && request.WithUnassignEmployeesFromItAgreement == false)
                {
                    finalresult.Succeeded = false;
                    finalresult.Errors.Add("The department is already having employees inside it, it's a risk to delete it now. If you are sure that you want to delete it then enable force delete.");
                    finalresult.NeedsApproval = true;
                    return finalresult;
                }
                else if (count != 0 && request.WithUnassignEmployeesFromItAgreement == true)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        // Unassign employees from the department
                        var employeeDepartments = await _dbcontext.EmployeeDepartmentsTable
                            .Where(ed => ed.Department_FK == department.Id)
                            .ToListAsync();
                        foreach (var empDept in employeeDepartments)
                        {
                            empDept.Department_FK = null; // Unassign the department
                        }
                        _dbcontext.EmployeeDepartmentsTable.UpdateRange(employeeDepartments);
                        _dbcontext.DepartmentsTable.Remove(department);

                        var log = new Audit_Logs
                        {
                            ActionType = "Deleting",
                            Notes = $"Hard deleted '{department.DepartmentName}' department",
                            PerformerAccount_FK = currentAdminID,
                            TargetEntity_FK = null
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                        await _dbcontext.SaveChangesAsync();
                        scope.Complete();
                        finalresult.Succeeded = true;
                        finalresult.Messages.Add($"Department {department.DepartmentName} was successfully deleted along with unassigning its employees from it.");
                        return finalresult;
                    }
                }
               else
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                    {
                     
                        _dbcontext.DepartmentsTable.Remove(department);
                        var log = new Audit_Logs
                        {
                            ActionType = "Deleting",
                            Notes = $"Soft deleted '{department.DepartmentName}' department",
                            PerformerAccount_FK = currentAdminID,
                            TargetEntity_FK = null
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                        await _dbcontext.SaveChangesAsync();
                        scope.Complete();
                        finalresult.Succeeded = true;
                        finalresult.Messages.Add($"Department was successfully deleted.");


                     
                        return finalresult;



                    }
                }
             

                // Step 4: Delete the user

            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, $"An error occurred while deleting the department with the GUID {request.DepartmentID}");

                finalresult.Succeeded = false;
                finalresult.Errors.Add("Couldn't delete the department from the system.");
                return finalresult;
            }
        }

        public async Task<IEnumerable<ReturnDepartmentsNames>> GetAllDepartments(string requesterID)
        {
          

            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var department = await _dbcontext.DepartmentsTable.Where(d => d.Manager_FK==null)
           .Select(ur => new ReturnDepartmentsNames
           {
            DepartmentName=ur.DepartmentName,
            Id=ur.Id
           }).ToListAsync();







            return department;

        }


        public async Task<FinalResult> AssignManagerToADepartment(string requesterId, AssignManagerToADepartmentRequest request)
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
             
                Guid requesteridmanager;
                try
                {
                     requesteridmanager = await _dbcontext.Users.Where(e => e.Id == requester.Id).Select(s => s.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }
                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to create a new department in the system.");
                    return finalResult;
                }

                var department = await _dbcontext.DepartmentsTable.Where(c => c.Id == request.DepartmentID).FirstOrDefaultAsync();
                if (department == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department was not found in the system.");
                    return finalResult;
                }
                if (department.Manager_FK != null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department {department.DepartmentName} is already having a manager assigned to it.");
                    return finalResult;
                }
                var Manager = await _dbcontext.EmployeesTable.Where(c => c.Id == request.ManagerID).FirstOrDefaultAsync();
                if (Manager == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Manager was not found in the system.");
                    return finalResult;
                }

                string managername = await _dbcontext.EmployeesTable.AsNoTracking()
               .Where(e => e.Id == Manager.Id)
              .Select(e => e.IdentityUserNA.PersonalInformationNA.FullName)
              .FirstOrDefaultAsync() ?? "Unknown Manager";
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    department.Manager_FK=Manager.Id;
                    var log = new Audit_Logs
                    {
                        ActionType = "Updating",
                        Notes= $"Assigned a department manager '{managername}' to a department called {department.DepartmentName}",
                        PerformerAccount_FK = requesteridmanager,
                        TargetEntity_FK=Manager.Id
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);
                
                        
                    
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }



                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Department manager '{managername}' was successfully assigned to the department '{department.DepartmentName}'.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during assigning a manager from to a department.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add($"We couldn't assign the manager to the department.");
                return finalResult;
            }
        }

        public async Task<IEnumerable<ReturnLoginHistoryForMyselfResponse>> ReturnLoginHistoryOfMine(
      string requesterID,
      ReturnLoginHistoryForMyselfRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                return null;

            var query = _dbcontext.LoginHistoryTable
                .Where(d => d.ExtendedIdentityUsersDesktop_FK == requester.Id);

            // Filter by year and month of LoggedAt
            query = query.Where(d => d.LoggedAt.Year == request.Year && d.LoggedAt.Month == request.Month);

            // Order by LoggedAt depending on ReturnItAs
            query = request.ReturnItAs
                ? query.OrderByDescending(d => d.LoggedAt)   // newest to oldest
                : query.OrderBy(d => d.LoggedAt);            // oldest to newest

            var LoginHistory = await query.Select(ur => new ReturnLoginHistoryForMyselfResponse
            {
                LoggedOn = ur.LoggedAt,
                DeviceTokenHashed = ur.DeviceTokenhashed,
                DeviceName = ur.DeviceName,
            }).ToListAsync();

            return LoginHistory;
        }
        public async Task<IEnumerable<ReturnLoginHistoryForManagerResponse>> ReturnLoginHistoryOfManager(
    string requesterID,
    ReturnLoginHistoryForManagerRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                return null;

            var query = _dbcontext.LoginHistoryTable
                .Where(d => d.ExtendedIdentityUsersDesktop_FK == request.ID);

            // Filter by year and month of LoggedAt
            query = query.Where(d => d.LoggedAt.Year == request.Year && d.LoggedAt.Month == request.Month);

            // Order by LoggedAt depending on ReturnItAs
            query = request.ReturnItAs
                ? query.OrderByDescending(d => d.LoggedAt)   // newest to oldest
                : query.OrderBy(d => d.LoggedAt);            // oldest to newest

            var LoginHistory = await query.Select(ur => new ReturnLoginHistoryForManagerResponse
            {
                LoggedOn = ur.LoggedAt,
                DeviceTokenHashed = ur.DeviceTokenhashed,
                DeviceName = ur.DeviceName,
            }).ToListAsync();

            return LoginHistory;
        }


        public async Task<IEnumerable<ReturnAllManagers>> GetAllManagers(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);

            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var Managers = await _dbcontext.UserRoles.Where(u => u.RoleNA.Name==AppRoles_Desktop.DepartmentManager || u.RoleNA.Name == AppRoles_Desktop.HRManager)
           .Select(ur => new ReturnAllManagers
           {
              FullName = ur.UserNA.PersonalInformationNA.FullName,
              ID=ur.UserNA.Id
           })
           .ToListAsync();



            return Managers;

        }

        public async Task<IEnumerable<ReturnAllManagersAndAdmins>> GetAllManagersAndAdmins(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);

            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var Managers = await _dbcontext.UserRoles.Where(u => u.RoleNA.Name == AppRoles_Desktop.DepartmentManager || u.RoleNA.Name == AppRoles_Desktop.HRManager || u.RoleNA.Name==AppRoles_Desktop.Admin)
           .Select(ur => new ReturnAllManagersAndAdmins
           {
               FullName = ur.UserNA.Id == requester.Id
            ? ur.UserNA.PersonalInformationNA.FullName + " (Self)"
            : ur.UserNA.PersonalInformationNA.FullName,
               RoleName = ur.RoleNA.Name,
               ID = ur.UserNA.Id
           })
           .ToListAsync();



            return Managers;

        }

        public async Task<IEnumerable<GetLogsForAPersonResponse>> GetLogsForAPerson(string requesterID, GetLogsForAPersonRequest request)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);

            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }
            var employeeRecord= _dbcontext.EmployeesTable.Where(e=>e.IdentityUsers_FK == request.TargetID).FirstOrDefault();
            if (employeeRecord == null)
            {
                return null;
            }

            var Logs = await _dbcontext.AuditLogsTable.Where(u => u.PerformerAccount_FK== employeeRecord.Id)
           .Select(ur => new GetLogsForAPersonResponse
           {
            ActionType=ur.ActionType,
            Performed_At=ur.Performed_At,
            Notes= ur.Notes,
            Targeted=ur.TargetEntityNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**No Target**"
           })
           .ToListAsync();



            return Logs;

        }

        public async Task<IEnumerable<ReturnEmployees>> GetAllEmployees(string requesterID)
        {
            //var user =await _userManager.FindByEmailAsync(normalizedEmail);

            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var Employees = await _dbcontext.UserRoles
         .Where(ur => ur.RoleNA.Name == AppRoles_Desktop.Employee)
         //.Where(ur =>
         //    ur.UserNA.EmployeesNA != null &&
         //    (
         //        !ur.UserNA.EmployeesNA.EmployeeToDepatment.Any() || // no records at all
         //        ur.UserNA.EmployeesNA.EmployeeToDepatment.All(dep => dep.Department_FK == null) // all records have null department
         //    )
         //)
         .Select(ur => new ReturnEmployees
         {
             FullName = ur.UserNA.PersonalInformationNA.FullName,
             ID = ur.UserNA.Id
            
         })
         .ToListAsync();




            return Employees;

        }

        public async Task<IEnumerable<ReturnDepartments>> GetDepartmentsForEmployees(string requesterID)
        {


            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var department = await _dbcontext.DepartmentsTable
           .Select(ur => new ReturnDepartments
           {
               DepartmentName = ur.DepartmentName,
               DepartmentManager=ur.ManagerNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**Doesn't Have A Manager Yet**",
               Id = ur.Id
           }).ToListAsync();







            return department;

        }


        public async Task<FinalResult> AssignEmployeeToADepartment(string requesterId, AssignEmployeeToADepartmentRequest request)
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

                Guid requesteridmanager;
                try
                {
                    requesteridmanager = await _dbcontext.Users.Where(e => e.Id == requester.Id).Select(s => s.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }
                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to create a new department in the system.");
                    return finalResult;
                }

                var department = await _dbcontext.DepartmentsTable.Where(c => c.Id == request.DepartmentID).FirstOrDefaultAsync();
                if (department == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department was not found in the system.");
                    return finalResult;
                }
               
                var Employee = await _dbcontext.Users.Where(c => c.Id == request.EmployeeID)
                    .Select(c=> new
                    {
                    ID = c.Id,
                    EmployeeID=c.EmployeesNA.Id,
                    FullName = c.PersonalInformationNA.FullName ?? "**No Name**",
                  
                    }
                    
                    
                    ).FirstOrDefaultAsync();
                
                if (Employee == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Employee was not found in the system.");
                    return finalResult;
                }

                var record = await _dbcontext.EmployeeDepartmentsTable
                   .Where(ed => ed.Employee_FK == Employee.EmployeeID && ed.Department_FK == department.Id)
                   .FirstOrDefaultAsync();
                if (record != null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Employee is already assigned to this department.");
                    return finalResult;
                }

                var isAssignedToDepartment = await _dbcontext.EmployeeDepartmentsTable
                    .AnyAsync(ed => ed.Employee_FK == Employee.EmployeeID && ed.Department_FK != null);
                if(isAssignedToDepartment==true)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"The employee is already assigned to another department.");
                    return finalResult;
                }

               

               
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var EmployeeDepartmentRecord = await _dbcontext.EmployeeDepartmentsTable.Where(ed => ed.Employee_FK == Employee.EmployeeID).FirstOrDefaultAsync();
                    if (EmployeeDepartmentRecord != null)
                    {
                        EmployeeDepartmentRecord.Department_FK = department.Id;
                    }
                    else
                    {
                        var addemployeedepartment = new Employee_Departments
                        {
                            Employee_FK = Employee.EmployeeID,
                            Department_FK = department.Id
                        };
                        await _dbcontext.EmployeeDepartmentsTable.AddAsync(addemployeedepartment);

                    }
                    var log = new Audit_Logs
                    {
                        ActionType = "Updating",
                        Notes = $"Assigned an employee '{Employee.FullName}' to a department '{department.DepartmentName}'",
                        PerformerAccount_FK = requesteridmanager,
                        TargetEntity_FK = Employee.EmployeeID
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);



                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }



                finalResult.Succeeded = true;
                finalResult.Messages.Add($"The employee '{Employee.FullName}' was successfully assigned to the department '{department.DepartmentName}'.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during assigning an employee to a department.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add($"We couldn't assign the employee to the department.");
                return finalResult;
            }
        }


        public async Task<GetDepartmentOfEmployeeResponse> GetDepartmentOFAnEmployees(string requesterID, GetDepartmentOfEmployeeRequest request)
        {

            GetDepartmentOfEmployeeResponse response = new GetDepartmentOfEmployeeResponse();
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            Guid TargetEmployee;
            try
            {
                TargetEmployee = await _dbcontext.Users.Where(e => e.Id == request.EmployeeID).Select(s => s.EmployeesNA.Id).FirstOrDefaultAsync();
            }

            catch
            {

                response.DepartmentName = null;
                response.GotDepartment = false;
                return response;
            }

            var departmentInfo = await _dbcontext.EmployeeDepartmentsTable
                .Where(c => c.Employee_FK == TargetEmployee && c.Department_FK != null)
                .Select(ur => new GetDepartmentOfEmployeeResponse
                {
                    GotDepartment = true,
                    DepartmentName = ur.DepartmentNA.DepartmentName ?? "**Doesn't got a name**",
                    DepartmentID=ur.DepartmentNA.Id
                })
                .FirstOrDefaultAsync();

            if (departmentInfo == null)
            {
                response.GotDepartment = false;
                response.DepartmentName = null;
                response.DepartmentID = Guid.Empty;
            }
            else
            {
                response.GotDepartment = true;
                response.DepartmentName = departmentInfo.DepartmentName;
                response.DepartmentID = departmentInfo.DepartmentID;
            }

            return response;







        }


        public async Task<FinalResult> UnassignEmployeeFromADepartment(string requesterId, UnassignEmployeeToADepartmentRequest request)
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

                Guid requesteridmanager;
                try
                {
                    requesteridmanager = await _dbcontext.Users.Where(e => e.Id == requester.Id).Select(s => s.EmployeesNA.Id).FirstOrDefaultAsync();
                }

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }
                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Admins are allowed to create a new department in the system.");
                    return finalResult;
                }

                var department = await _dbcontext.DepartmentsTable.Where(c => c.Id == request.DepartmentID).FirstOrDefaultAsync();
                if (department == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Department was not found in the system.");
                    return finalResult;
                }

                var Employee = await _dbcontext.Users.Where(c => c.Id == request.EmployeeID)
                    .Select(c => new
                    {
                        ID = c.Id,
                        EmployeeID = c.EmployeesNA.Id,
                        FullName = c.PersonalInformationNA.FullName ?? "**No Name**",

                    }


                    ).FirstOrDefaultAsync();

                if (Employee == null)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add($"Employee was not found in the system.");
                    return finalResult;
                }

                //var isAssignedToDepartment = await _dbcontext.EmployeeDepartmentsTable
                //    .AnyAsync(ed => ed.Employee_FK == Employee.EmployeeID && ed.Department_FK != null);
                //if (isAssignedToDepartment == true)
                //{
                //    finalResult.Succeeded = false;
                //    finalResult.Errors.Add($"The employee is already assigned to another department.");
                //    return finalResult;
                //}

                //var record = _dbcontext.EmployeeDepartmentsTable
                //    .Where(ed => ed.Employee_FK == Employee.EmployeeID && ed.Department_FK == department.Id)
                //    .AnyAsync();
                //if (record != null)
                //{
                //    finalResult.Succeeded = false;
                //    finalResult.Errors.Add($"Employee is already assigned to this department.");
                //    return finalResult;
                //}


                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var EmployeeDepartmentRecord = await _dbcontext.EmployeeDepartmentsTable.Where(ed => ed.Employee_FK == Employee.EmployeeID && ed.Department_FK==request.DepartmentID).FirstOrDefaultAsync();
                    if (EmployeeDepartmentRecord == null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add($"The employee is not assigned to this department.");
                        return finalResult;
                    }
                    else
                    {
                        EmployeeDepartmentRecord.Department_FK = null;
                   

                    }
                    var log = new Audit_Logs
                    {
                        ActionType = "Updating",
                        Notes = $"Unassigned an employee '{Employee.FullName}' from a department '{department.DepartmentName}'",
                        PerformerAccount_FK = requesteridmanager,
                        TargetEntity_FK = Employee.EmployeeID
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);



                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }



                finalResult.Succeeded = true;
                finalResult.Messages.Add($"The employee '{Employee.FullName}' was successfully Unassigned from the department '{department.DepartmentName}'.");
                return finalResult;
            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error occurred during Unassigning an employee to a department.");
                finalResult.Succeeded = false;
                finalResult.Errors.Add($"We couldn't Unassign the employee from the department.");
                return finalResult;
            }
        }

        public async Task<GetCompanyName> GetCompanyName(string requesterId)
        {


            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    return null;
                }

                //var requesterRoles = await _userManager.GetRolesAsync(requester);
                //if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                //{

                //    return null;
                //}



                var result = await _dbcontext.GettingStartedTable
                .Select(x => new GetCompanyName
                {
                    CompanyName=x.CompanyName ?? "**No Name Defined**"
                }).FirstOrDefaultAsync();


                return result;




            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error while getting the company name.");
                return null;
            }
        }


        public async Task<PersonalInformationReturning> GetAllPersonalData(string requesterId)
        {


            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    return null;
                }

                //var requesterRoles = await _userManager.GetRolesAsync(requester);
                //if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                //{

                //    return null;
                //}



                var result = await _dbcontext.UserRoles.Where(r=>r.UserNA.Id==requester.Id)
                .Select(x => new PersonalInformationReturning
                {
                    FullName = x.UserNA.PersonalInformationNA.FullName ?? "**No Name Defined**",
                    Age=x.UserNA.PersonalInformationNA.Age,
                    Gender=x.UserNA.PersonalInformationNA.Gender,
                    Date_Of_Birth=x.UserNA.PersonalInformationNA.DateOfBirth,
                    Living_On_Primary_Place=x.UserNA.PersonalInformationNA.LivingOnPrimaryPlace==true ? "Yes" : "No",
                    Nationality=x.UserNA.PersonalInformationNA.Nationality!="" ?  x.UserNA.PersonalInformationNA.Nationality : "**No Nationality Defined**",
                    RoleName=x.RoleNA.Name,
                    RoleColor=x.RoleNA.RoleColorCode,
                    AccountStatus=x.UserNA.EmployeesNA.Status,
                    AccountCreatedDate = x.UserNA.CreatedAt,
                    HireDate=x.UserNA.EmployeesNA.HireDate,
                    AccountAgeInDays = DateOnly.FromDateTime(DateTime.Now).DayNumber - x.UserNA.CreatedAt.DayNumber


                }).FirstOrDefaultAsync();


                return result;




            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error while getting the company name.");
                return null;
            }
        }

        public async Task<ReturnUserRole> GetRole(string requesterId)
        {
            ReturnUserRole result = new ReturnUserRole();

            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    return null;
                }



                var role = await _dbcontext.UserRoles.Where(r => r.UserNA.Id == requester.Id)
                .Select(x => new ReturnUserRole
                {
                   RoleName=x.RoleNA.Name ?? ""
                }).FirstOrDefaultAsync();

                if(role==null)
                {
                    return null;
                }
                else
                {
                    result.RoleName = role.RoleName;
                }
                return result;




            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error while getting the company name.");
                return null;
            }
        }

        public async Task<UpdateCompanyNameResponse> UpdateCompanyName(string requesterId, UpdateCompanyNameRequest request)
        {


            try
            {
                var result = new UpdateCompanyNameResponse();

                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    return null;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {

                    return null;
                }
                string changeTo = request.CompanyName.Trim();


                int affectedRows = await _dbcontext.GettingStartedTable.ExecuteUpdateAsync(c => c.SetProperty(x => x.CompanyName, changeTo));

                if (affectedRows > 0)
                {
                    result.NewCompanyName = changeTo;
                    return result;
                }
                else
                {
                    return null;
                }
               




            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error while getting the company name.");
                return null;
            }
        }

        public async Task<IEnumerable<ReturnSystemRoles>> GetAllSystemRoles(string requesterID)
        {


            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
            {

                return null;
            }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
            {

                return null;
            }

            var Roles = await _dbcontext.Roles
           .Select(ur => new ReturnSystemRoles
           {
             RoleName=ur.Name,
             RoleColorCode = ur.RoleColorCode,
             RoleID=ur.Id,
           }).ToListAsync();

          if(Roles.Count==0)
            {
                return null;
            }





            return Roles;

        }

        public async Task<object> UpdateRoleColor(string requesterId, RoleColorChangeRequest request)
        {


            try
            {
             
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    return null;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Admin))
                {

                    return null;
                }
             


                int affectedRows = await _dbcontext.Roles.Where(c=>c.Id==request.ID).ExecuteUpdateAsync(c => c.SetProperty(x => x.RoleColorCode, request.ColorCode));

                if (affectedRows > 0)
                {

                    return new object();
                }
                else
                {
                    return null;
                }





            }
            catch (Exception ex)
            {
                _loggerr.LogError(ex, "Error while getting the company name.");
                return null;
            }
        }





    }
}
