using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Helpers_Services.SignleR_Logic;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Employee;

namespace Proz_WebApi.Services.DesktopServices
{
    public class EmployeeLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<AdminLogicService> _logger;
        private readonly IHubContext<MainHub> _hub;
        public EmployeeLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager
            , ApplicationDbContext_Desktop dbcontext, ILogger<AdminLogicService> loggerr, IHubContext<MainHub> hub)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
            _hub = hub;
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
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
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
               
                return null;
            }
        }


        public async Task<FinalResult> CreateANewfeedbackRequest(string requesterId, CreateANewFeedbackRequest_Request request)
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
 

                Guid EmployeeID;
                Guid SenderID;
                Guid? departmentmanager;
                try
                {

                    EmployeeID = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();

                    Guid? department = await _dbcontext.EmployeeDepartmentsTable
                   .Where(c => c.Employee_FK == EmployeeID && c.Department_FK!=null)
                   .Select(c => c.Department_FK).FirstOrDefaultAsync();
                    if (department == null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Employee is not inside a department.");
                        return finalResult;
                    }
                     departmentmanager = await _dbcontext.DepartmentsTable.Where(c => c.Id == department).Select(c => c.Manager_FK).FirstOrDefaultAsync();
                    if (departmentmanager == null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Employee is assigned to a department but this department doesn't have a manager to send the request to.");
                        return finalResult;
                    }

                    SenderID = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Employee_FK == EmployeeID && c.Department_FK==department).Select(c => c.Id).FirstOrDefaultAsync();




                }
      

                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Only Employee are allowed to make Feedback requests.");
                    return finalResult;
                }

                Notifications? notification;
                Guid manageriduser= await _dbcontext.EmployeesTable.Where(c => c.Id == departmentmanager)
                 .Select(s => s.IdentityUserNA.Id).FirstOrDefaultAsync();
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var employeename = await _dbcontext.Users.Where(c => c.Id == requester.Id)
                    .Select(c => c.PersonalInformationNA.FullName).FirstOrDefaultAsync();
                    if (string.IsNullOrEmpty(employeename))                                   //<<this is new thing!
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Employee name is not valid.");
                        return finalResult;
                    }
                    await _dbcontext.FeedbacksTable.AddAsync(new Feedbacks
                    {
                        FeedbackTitle = request.FeedbackTitle,
                        FeedbackDescription = request.FeedbackDescription,
                        FeedbackType_FK = request.FeedbackType,
                        RequesterEmployee_FK = SenderID,
                      
                        

                    });
                    var feedbackTypeName = await _dbcontext.FeedbacksTypesTable.Where(f=>f.id == request.FeedbackType)
                        .Select(f => f.FeedbackType).FirstOrDefaultAsync();
                    notification = new Notifications()
                   {
                       Title = "New Feedback Request",
                       Message = $"{employeename} has requested a feedback of a type {feedbackTypeName}.",
                       Created_At = DateTime.UtcNow,
                       Type = "Feedback",
                       Priority = "Low",
                       Target_FK = (Guid)departmentmanager,
                   };
                    await _dbcontext.NotificationsTable.AddAsync(notification);
                   
                  

                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }

                await _hub.Clients.User(manageriduser.ToString()).SendAsync("NewNotification", notification);
                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Your Feedback request was successfully sent to your manager, wait for an answer.");
                return finalResult;
            }
            catch (Exception ex)
            {

                finalResult.Succeeded = false;
                finalResult.Errors.Add("Couldn't send the feedback request, please wait and try again later.");
                return finalResult;
            }
        }

        public async Task<IEnumerable<RetrunFeedbacksInformation>> GetMyFeedbacks(string requesterId)
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
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {

                    return null;
                }
                
                Guid? employeeid= await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                if (employeeid == null)
                {
                    return null;
                }
                Guid? row = await _dbcontext.EmployeeDepartmentsTable.Where(x=>x.Employee_FK==employeeid).Select(x=>x.Id).FirstOrDefaultAsync();
                if (row == null)
                {
                    return null;
                }

                var result = await _dbcontext.FeedbacksTable.Where(x=>x.RequesterEmployee_FK== row)
                .Select(x => new RetrunFeedbacksInformation
                {
                FeedbackId=x.id,
                    FeedbackDescription = x.FeedbackDescription,
                    FeedbackTitle = x.FeedbackTitle,
                    CreatedAt = x.LastUpdated,
                    FeedbackType = x.FeedbackTypeNA.FeedbackType,
                    FeedbackAnswer = x.FeedbacksAnswerNA != null ? x.FeedbacksAnswerNA.Answer : "**No Answer yet**",

                    AnsweredIn = x.FeedbacksAnswerNA != null ? x.FeedbacksAnswerNA.LastUpdated : (DateTime?)null,

                    AnsweredBy = x.FeedbacksAnswerNA != null &&
                     x.FeedbacksAnswerNA.EmployeeNA != null &&
                     x.FeedbacksAnswerNA.EmployeeNA.IdentityUserNA != null &&
                     x.FeedbacksAnswerNA.EmployeeNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.FeedbacksAnswerNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.FeedbacksAnswerNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**No Answer yet**"

                }).ToListAsync();


                return result;




            }
            catch (Exception ex)
            {
              
                return null;
            }
        }

        public async Task<RemoveMyFeedbackResponse> RemoveMyFeedback(string requesterId, RemoveMyFeedbackRequest request)
        {
            var finalresult = new RemoveMyFeedbackResponse();

            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    finalresult.Error= "Requester is invalid.";
                    finalresult.Success = false;
                    return finalresult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {

                    finalresult.Error = "Requester is invalid.";
                    finalresult.Success = false;
                    return finalresult;
                }

                var feedback = await _dbcontext.FeedbacksTable.Where(x=>x.id == request.FeedbackID).FirstOrDefaultAsync();
                if(feedback == null)
                {
                    finalresult.Error = "Feedback is not located.";
                    finalresult.Success = false;
                    return finalresult;
                }

             

                _dbcontext.Remove(feedback);
               await _dbcontext.SaveChangesAsync();
              

                finalresult.Message= "Your feedback request was successfully removed.";
                finalresult.Success = true;
                return finalresult;




            }
            catch (Exception ex)
            {

                finalresult.Error = "Couldn't remove your feedback.";
                finalresult.Success = false;
                return finalresult;
            }
        }


        public async Task<FinalResult> CreateANewLeaveRequest(string requesterId, CreateANewLeaveRequest_Request request)
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


                Guid EmployeeID;
                Guid SenderID;

                try
                {

                    EmployeeID = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();

                    Guid? department = await _dbcontext.EmployeeDepartmentsTable
                   .Where(c => c.Employee_FK == EmployeeID && c.Department_FK != null)
                   .Select(c => c.Department_FK).FirstOrDefaultAsync();
                    if (department == null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Employee is not inside a department.");
                        return finalResult;
                    }
                    Guid? departmentmanager = await _dbcontext.DepartmentsTable.Where(c => c.Id == department).Select(c => c.Manager_FK).FirstOrDefaultAsync();
                    if (departmentmanager == null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Employee is assigned to a department but this department doesn't have a manager to send the request to.");
                        return finalResult;
                    }

                    SenderID = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Employee_FK == EmployeeID && c.Department_FK == department).Select(c => c.Id).FirstOrDefaultAsync();



                }


                catch
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester");
                    return finalResult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("Invalid requester.");
                    return finalResult;
                }

                bool Exisiting = await _dbcontext.LeaveRequestsTable.AnyAsync(x => x.Requester_Employee_FK == SenderID && x.Completed == false);


                if (Exisiting==true)
                {
                    finalResult.Succeeded = false;
                    finalResult.Errors.Add("There is already an active leave request, please wait until it get completed or you can delete it to request your new one.");
                    return finalResult;
                }

                var requesterInformation = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Id == SenderID)
                 .Select(s => new
                 {
                     employeename = s.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName,
                     department = s.DepartmentNA.DepartmentName,
                     managerID = s.DepartmentNA.Manager_FK,
                     managerIDUser = s.DepartmentNA.ManagerNA.IdentityUserNA.Id
                 }).FirstOrDefaultAsync();
                Notifications? notification;
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                 
                   
                    if(requesterInformation==null)
                    {
                        finalResult.Succeeded = false;
                        finalResult.Errors.Add("Requester information is not valid.");
                        return finalResult;
                    }

                    await _dbcontext.LeaveRequestsTable.AddAsync(new LeaveRequests
                    {
                        Requester_Employee_FK = SenderID,
                        StartDate = DateOnly.ParseExact(request.FromDATE, "yyyy-MM-dd"),
                        EndDate = DateOnly.ParseExact(request.ToDATE, "yyyy-MM-dd"),
                        Reason=request.Reason


                    });

                    notification = new Notifications
                    {
                        Title = "New Leave Request",
                        Message = $"{requesterInformation.employeename} has requested a leave request from your '{requesterInformation.department}' department.",
                        Created_At = DateTime.UtcNow,
                        Type = "Leave Request",
                        Priority = "Medium",
                        Target_FK = (Guid)requesterInformation.managerID,
                    };
               

                    await _dbcontext.NotificationsTable.AddAsync(notification);

                  
                  await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
              
                await _hub.Clients.User(requesterInformation.managerIDUser.ToString()).SendAsync("NewNotification", notification);

                finalResult.Succeeded = true;
                finalResult.Messages.Add($"Your Leave request was successfully sent to your manager, wait for an answer.");
                return finalResult;
            }
            catch (Exception ex)
            {

                finalResult.Succeeded = false;
                finalResult.Errors.Add("Couldn't send the Leave request, please wait and try again later.");
                return finalResult;
            }
        }

        public async Task<IEnumerable<ReturnLeaveRequestsInformation>> GetMyLeaveRequests(string requesterId)
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
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {

                    return null;
                }

                Guid? employeeid = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                if (employeeid == null)
                {
                    return null;
                }
                Guid? row = await _dbcontext.EmployeeDepartmentsTable.Where(x => x.Employee_FK == employeeid).Select(x => x.Id).FirstOrDefaultAsync();
                if (row == null)
                {
                    return null;
                }

                var result = await _dbcontext.LeaveRequestsTable.Where(x => x.Requester_Employee_FK == row)
                .Select(x => new ReturnLeaveRequestsInformation
                {
                    LeaveRequestId = x.Id,
                    Reason = x.Reason,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,

                    SentAt = x.LastUpdate,
                    DMAnsweredAt = x.LastUpdateDM ?? (DateTime?)null,
                    HRMAnsweredAt = x.LastUpdateHR ?? (DateTime?)null,
                    CompletedAt=x.Decision_At ?? (DateTime?)null,

                    DepartmentManagerComment = x.DepartmentManagerComment ?? "**No Answer yet**",
                    FinalStatusComment = x.FinalStatus_Comment ?? "**No Answer yet**",

                    RequesterStatus = x.RequesterStatus,
                    DMStatus = x.DMStatus ?? "Waiting",
                    FinalStatus = x.FinalStatus ?? "Waiting",

                    Completed = x.Completed ?? (bool?)null,

                    HasSanctions = x.HasSanctions ?? (bool?)null,
                    AgreedOn = x.AgreedOn ?? (bool?)null,

                    DMName = x.DepartmentManagerNA != null &&
                     x.DepartmentManagerNA.IdentityUserNA != null &&
                     x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**No Answer yet**",

                    HRMName = x.HandlerEmployeeNA != null &&
                     x.HandlerEmployeeNA.IdentityUserNA != null &&
                     x.HandlerEmployeeNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.HandlerEmployeeNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.HandlerEmployeeNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**No Answer yet**",

                 
                }).ToListAsync();


                return result;




            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<RemoveMyLeaveResponse> RemoveMyLeaveRequest(string requesterId, RemoveMyLeaveRequest request)
        {
            var finalresult = new RemoveMyLeaveResponse();

            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {

                    finalresult.Error = "Requester is invalid.";
                    finalresult.Success = false;
                    return finalresult;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {

                    finalresult.Error = "Requester is invalid.";
                    finalresult.Success = false;
                    return finalresult;
                }

                var LeaveRequest = await _dbcontext.LeaveRequestsTable.Where(x => x.Id == request.LeaveRequestID).FirstOrDefaultAsync();
                if (LeaveRequest == null)
                {
                    finalresult.Error = "Leave Request is not located.";
                    finalresult.Success = false;
                    return finalresult;
                }



                _dbcontext.Remove(LeaveRequest);
                await _dbcontext.SaveChangesAsync();


                finalresult.Message = "Your Leave request was successfully removed.";
                finalresult.Success = true;
                return finalresult;




            }
            catch (Exception ex)
            {

                finalresult.Error = "Couldn't remove your leave request.";
                finalresult.Success = false;
                return finalresult;
            }
        }


        public async Task<AgreeOnLeaveRequestDecisionResponse> AgreeOnLeaveRequestRules(string requesterId, AgreeOnLeaveRequestDecisionRequest request)
        {
            var finalResult = new AgreeOnLeaveRequestDecisionResponse();

            try
            {
                // Get the admin making the request
                var requester = await _userManager.FindByIdAsync(requesterId);
                if (requester == null)
                {
                    finalResult.Success = false;
                    finalResult.Error = "invalid Requester.";
                    return finalResult;
                }


             

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {
                    finalResult.Success = false;
                    finalResult.Error = "invalid Requester.";
                    return finalResult;
                }

                var LeaveRequest = await _dbcontext.LeaveRequestsTable.Where(x => x.Id == request.LeaveRequestID).FirstOrDefaultAsync();
                if(LeaveRequest == null)
                {
                    finalResult.Success = false;
                    finalResult.Error = "Leave Request is not located.";
                    return finalResult;
                }

                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (LeaveRequest.Completed==true || LeaveRequest.HasSanctions==false || LeaveRequest.DMStatus=="Waiting")
                    {
                        finalResult.Success = false;
                        finalResult.Error = "Leave Request is already completed or it doesn't require from you to agree about anything.";
                        return finalResult;
                    }   
                    LeaveRequest.AgreedOn = request.Agreed;
                    LeaveRequest.Completed = true;
                    LeaveRequest.Decision_At = DateTime.UtcNow;
                    LeaveRequest.RequesterStatus = "Done";
                    if (request.Agreed == true)
                    {
                 
                        LeaveRequest.FinalStatus = "Accepted";
                    }
                    else
                    {
                        LeaveRequest.FinalStatus = "Rejected";
                    }


                        await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }


                finalResult.Success = true;
                finalResult.Message = "Your Leave request was successfully sent to your manager, wait for an answer.";
                return finalResult;
            }
            catch (Exception ex)
            {

                finalResult.Success = false;
                finalResult.Error = "Couldn't send the Leave request, please wait and try again later.";
                return finalResult;
            }
        }



        public async Task<IEnumerable<ReturnPerformanceRecordsResponse>> GetMyPerformanceRecords(string requesterId, ReturnPerformanceRecordsListRequest request)
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
                if (!requesterRoles.Contains(AppRoles_Desktop.Employee))
                {

                    return null;
                }

                Guid? employeeid = await _dbcontext.Users.Where(c => c.Id == requester.Id).Select(c => c.EmployeesNA.Id).FirstOrDefaultAsync();
                if (employeeid == null)
                {
                    return null;
                }
                Guid? row = await _dbcontext.EmployeeDepartmentsTable.Where(x => x.Employee_FK == employeeid).Select(x => x.Id).FirstOrDefaultAsync();
                if (row == null)
                {
                    return null;
                }

                var result = await _dbcontext.PerformanceRecorderTable.Where(x => x.EmployeeDepartment_FK == row && x.CreatedAt.Month==request.Month)
                .Select(x => new ReturnPerformanceRecordsResponse
                {
                    CreatedAt = x.CreatedAt,
                    
                    PerformanceRating = x.PerformanceRating,
                    ReviewerComment = x.ReviewerComment ?? "**No Comment**",
                    Reviewer = x.ReviewerNA != null &&
                     x.ReviewerNA.IdentityUserNA != null &&
                     x.ReviewerNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.ReviewerNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.ReviewerNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**Reviewer has no name**"


                }).ToListAsync();


                return result;




            }
            catch (Exception ex)
            {

                return null;
            }
        }




    }
}
