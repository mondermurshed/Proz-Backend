using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Exceptions;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.Dto.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.Admin;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;
using Proz_WebApi.Models.DesktopModels.DTO.LoginHistoryDTOs;



//using Proz_WebApi.Models.DesktopModels.Dto.DepartmentManager;
using Zxcvbn;

namespace Proz_WebApi.Services.DesktopServices
{
    public class DepartmentManagerLogicService
    {
        private readonly UserManager<ExtendedIdentityUsersDesktop> _userManager;
        private readonly RoleManager<ExtendedIdentityRolesDesktop> _roleManager;
        private readonly ApplicationDbContext_Desktop _dbcontext;
        private readonly ILogger<DepartmentManagerLogicService> _logger;


        public DepartmentManagerLogicService(UserManager<ExtendedIdentityUsersDesktop> userManager, RoleManager<ExtendedIdentityRolesDesktop> roleManager, ApplicationDbContext_Desktop dbcontext, ILogger<DepartmentManagerLogicService> loggerr)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
            _logger = loggerr;
        }

        public async Task<IEnumerable<ReturnLoginHistory>> ReturnMyLoginHistoty(string requesterID)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.Employee) &&  !requesterRoles.Contains(AppRoles_Desktop.DepartmentManager) && !requesterRoles.Contains(AppRoles_Desktop.HRManager))
                return null;




            var LoginHistory = await _dbcontext.LoginHistoryTable.Where(c => c.ExtendedIdentityUsersDesktop_FK == requester.Id).Select(ur => new ReturnLoginHistory
            {
                LoggedOn = ur.LoggedAt,
                DeviceTokenHashed = ur.DeviceTokenhashed,
                DeviceName = ur.DeviceName,
            }).Take(250).OrderByDescending(r=>r.LoggedOn).ToListAsync();

            return LoginHistory;
        }


        public async Task<IEnumerable<ReturnAllDepartments>> ReturnMyDepartments(string requesterID)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c=>c.IdentityUsers_FK==requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var Departments = await _dbcontext.DepartmentsTable.Where(c => c.Manager_FK == managerid).Select(ur => new ReturnAllDepartments
            {
                DepartmentID = ur.Id,
                DepartmentName = ur.DepartmentName
            }).ToListAsync();

            if (Departments == null || Departments.Count == 0)
                return null;
            else
            return Departments;

        }

        public async Task<IEnumerable<ReturnFeedbacksResponse>> ReturnFeedbacksOfADepartment(string requesterID, ReturnFeedbacksRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var Feedbacks = await _dbcontext.FeedbacksTable.Where(c => c.EmployeeNA.Department_FK == request.Department && c.IsCompleted==false).Select(ur => new ReturnFeedbacksResponse
            {
             FeedbackID=ur.id,
             FeedbackDescription = ur.FeedbackDescription,
                FeedbackTitle = ur.FeedbackTitle,
                FeedbackTypeName = ur.FeedbackTypeNA.FeedbackType,
                CreatedAt = ur.LastUpdated,
                RequesterEmployeeName= ur.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName
            }).ToListAsync();

            if (Feedbacks == null || Feedbacks.Count == 0)
                return null;
            else
                return Feedbacks;

        }

        public async Task<AddAnAnswerForAFeedbackResponse> CreateAnAnswerForAFeedback(string requesterID, AddAnAnswerForAFeedbackRequest request)
        {
            var resultfinal = new AddAnAnswerForAFeedbackResponse();
            try
            {
         

                var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                {
                    resultfinal.Error = "The Requester is unknown";
                   resultfinal.Success = false;
                    return resultfinal;
                }

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }


                Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
               
                    var feedback = await _dbcontext.FeedbacksTable.FirstOrDefaultAsync(c => c.id == request.TargetedFeedback);
                if (feedback == null)
                {
                    resultfinal.Error = "The targeted feedback is not found";
                    resultfinal.Success = false;
                    return resultfinal;
                }
                if (feedback.IsCompleted == true)
                {
                    resultfinal.Error = "This feedback is already answered.";
                    resultfinal.Success = false;
                    return resultfinal;
                }
                    var employee = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Id == feedback.RequesterEmployee_FK)
                    .Select(c => new
                    {
                        ID = c.EmployeeNA.IdentityUserNA.Id,
                        EmployeeID = c.EmployeeNA.Id,
                        FullName = c.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**No Name**",

                    }
                    ).FirstOrDefaultAsync();

                    feedback.IsCompleted = true;
                _dbcontext.Update(feedback);

                var feedbackanswer = new Feedbacks_Answers
           {
               Answer = request.Answer,
               Feedback_FK = request.TargetedFeedback,
               RespondentAccount_FK = managerid.Value,
              
           };

                    var log = new Audit_Logs
                    {
                        ActionType = "Creating",
                        Notes = $"Answered a feedback request for the employee '{employee.FullName}'",
                        PerformerAccount_FK = managerid,
                        TargetEntity_FK = employee.EmployeeID
                    };
                    await _dbcontext.AuditLogsTable.AddAsync(log);

                    await _dbcontext.AddAsync(feedbackanswer);
                   
                await _dbcontext.SaveChangesAsync();
                 scope.Complete();
                }
                resultfinal.Success = true; 
                resultfinal.Messagee= "Your answer has been added successfully to your employee.";
                return resultfinal;
            }
            catch
            {
                resultfinal.Error = "An error occurred while processing your request. Please try again later.";
                resultfinal.Success = false;
                return resultfinal;
            }
         }

        public async Task<IEnumerable<ReturnFinishedFeedbacksResponse>> ReturnFinishedFeedbacksOfADepartment(string requesterID, ReturnFinishedFeedbacksRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var Feedbacks = await _dbcontext.FeedbacksTable.Where(c => c.EmployeeNA.Department_FK == request.Department && c.IsCompleted == true).Select(ur => new ReturnFinishedFeedbacksResponse
            {
                FeedbackID = ur.id,
                FeedbackDescription = ur.FeedbackDescription,
                FeedbackTitle = ur.FeedbackTitle,
                FeedbackTypeName = ur.FeedbackTypeNA.FeedbackType,
                CreatedAt = ur.LastUpdated,
                AnsweredAt= ur.FeedbacksAnswerNA.LastUpdated,
                RequesterEmployeeName = ur.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName,
                MyAnswer = ur.FeedbacksAnswerNA.Answer ?? "**No Answer**"

            }).ToListAsync();

            if (Feedbacks == null || Feedbacks.Count == 0)
                return null;
            else
                return Feedbacks;

        }

        public async Task<IEnumerable<ReturnMyEmployeesLeaveRequests>> ReturnLeaveRequestsOfADepartment(string requesterID, ReturnMyEmployeesLeaveRequests_Request_ request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var LeaveRequests = await _dbcontext.LeaveRequestsTable.Where(c => c.EmployeeNA.Department_FK == request.Department && c.DepartmentManager_FK==null).Select(ur => new ReturnMyEmployeesLeaveRequests
            {
                LeaveRequestID=ur.Id,
                employeeName = ur.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName,
                From = ur.StartDate,
                To = ur.EndDate,
                Reason = ur.Reason
            }).ToListAsync();

            if (LeaveRequests == null || LeaveRequests.Count == 0)
                return null;
            else
                return LeaveRequests;

        }


        public async Task<AddAnAnswerForALeaveRequestResponse> CreateAnAnswerForALeaveRequest(string requesterID, LeaveRequestAcceptRejectRequest request)
        {
            var resultfinal = new AddAnAnswerForALeaveRequestResponse();
            try
            {


                var requester = await _userManager.FindByIdAsync(requesterID);
                if (requester == null)
                {
                    resultfinal.Error = "The Requester is unknown";
                    resultfinal.Success = false;
                    return resultfinal;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }


                Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
                if (managerid == null)
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {

                    var LeaveRequest = await _dbcontext.LeaveRequestsTable.FirstOrDefaultAsync(c => c.Id == request.LeaveRequestID);
                    if (LeaveRequest == null)
                    {
                        resultfinal.Error = "The targeted Leave request is not found";
                        resultfinal.Success = false;
                        return resultfinal;
                    }
                    if (LeaveRequest.DepartmentManager_FK != null)
                    {
                        resultfinal.Error = "This Leave Request is already answered.";
                        resultfinal.Success = false;
                        return resultfinal;
                    }
                    LeaveRequest.DepartmentManager_FK = managerid;
                    LeaveRequest.DepartmentManagerComment = request.Comment;
                    LeaveRequest.LastUpdateDM = DateTime.UtcNow;
                    LeaveRequest.LastUpdateHR = null;
                    LeaveRequest.DMStatus = "Accepted";
                 
                    if (request.Accept == false)
                    {
                        LeaveRequest.DMStatus = "Rejected";
                        LeaveRequest.RequesterStatus = "Done";
                        LeaveRequest.Completed = true;
                        LeaveRequest.FinalStatus = "Done";
                        LeaveRequest.FinalStatus_Comment = "**Completed by the Department Manager**";
                        LeaveRequest.Decision_At = DateTime.UtcNow;
                        
                    }
                    _dbcontext.Update(LeaveRequest);

                    var employee = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Id == LeaveRequest.Requester_Employee_FK)
                    .Select(c => new
                    {
                        ID = c.EmployeeNA.IdentityUserNA.Id,
                        EmployeeID = c.EmployeeNA.Id,
                        FullName = c.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**No Name**",
                    
                    }
                    ).FirstOrDefaultAsync();
                    if (request.Accept==true)
                    {
                        var log = new Audit_Logs
                        {
                            ActionType = "Creating",
                            Notes = $"Accepted a leave request of the employee '{employee.FullName}'",
                            PerformerAccount_FK = managerid,
                            TargetEntity_FK = employee.EmployeeID
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                    }
                    else
                    {
                        var log = new Audit_Logs
                        {
                            ActionType = "Creating",
                            Notes = $"Rejected a leave request of the employee '{employee.FullName}'",
                            PerformerAccount_FK = managerid,
                            TargetEntity_FK = employee.EmployeeID
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                    }



                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
                if (request.Accept==true)
                {
                    resultfinal.Success = true;
                    resultfinal.Messagee = "The leave request was accepted successfully from your end.";
                    return resultfinal;
                }
        
                {
                    resultfinal.Success = true;
                    resultfinal.Messagee = "The leave request was rejected successfully.";
                    return resultfinal;
                }
            }
            catch
            {
                resultfinal.Error = "An error occurred while processing your request. Please try again later.";
                resultfinal.Success = false;
                return resultfinal;
            }
        }

        public async Task<IEnumerable<ReturnFinishedLeaveRequestsResponse>> ReturnFinishedLeaveRequestsOfADepartment(string requesterID, ReturnFinishedLeaveRequestsRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var LeaveRequests = await _dbcontext.LeaveRequestsTable.Where(x => x.EmployeeNA.Department_FK == request.Department && x.DepartmentManager_FK != null).Select(x => new ReturnFinishedLeaveRequestsResponse
            {


                LeaveRequestID = x.Id,
                Reason = x.Reason,
                MyAnswer = x.DepartmentManagerComment ?? "**No Answer**",
                From = x.StartDate,
                To = x.EndDate,

                LeaveRequestOLD = x.LastUpdate,
                AnswerOld = x.LastUpdateDM,

                SenderName = x.DepartmentManagerNA != null &&
                     x.DepartmentManagerNA.IdentityUserNA != null &&
                     x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**Name Is Not Defined**",

                employeeName = x.EmployeeNA != null &&
                     x.EmployeeNA.EmployeeNA != null &&
                     x.EmployeeNA.EmployeeNA.IdentityUserNA != null &&
                     x.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA != null &&
                     !string.IsNullOrEmpty(x.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName)
                     ? x.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName
                     : "**Name Is Not Defined**",
                    
                    Accepted=x.DMStatus=="Accepted" ? true : false
            


            }).ToListAsync();

            if (LeaveRequests == null || LeaveRequests.Count == 0)
                return null;
            else
                return LeaveRequests;

        }


        public async Task<IEnumerable<ReturnPerformanceRecordsResponse>> ReturnEmployeesWithIDsDepartment(string requesterID, ReturnPerformanceRecordsRequest request)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var Employees = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Department_FK == request.DepartmentID).Select(ur => new ReturnPerformanceRecordsResponse
            {
                EmployeeID = ur.Id,
                EmployeeName = ur.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName

            }).ToListAsync();
            //var Employees = await _dbcontext.PerformanceRecorderTable.Where(c => c.EmployeeDepartmentNA.Department_FK==request.DepartmentID).Select(ur => new ReturnPerformanceRecordsResponse
            //{
            //    PerformanceRecordID = ur.Id,
            //    EmployeeName = ur.EmployeeDepartmentNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName,
            //    PerformanceRating=ur.PerformanceRating,
            //    PerformanceComment=ur.ReviewerComment

            //}).ToListAsync();

            if (Employees == null || Employees.Count == 0)
                return null;
            else
                return Employees;

        }


        public async Task<SubmitPerformanceAnswerResponse> CreateAnAnswerForPerformance(string requesterID, SubmitPerformanceAnswerRequest request)
        {
            var resultfinal = new SubmitPerformanceAnswerResponse();
            try
            {


                var requester = await _userManager.FindByIdAsync(requesterID);
                if (requester == null)
                {
                    resultfinal.Error = "The Requester is unknown";
                    resultfinal.Success = false;
                    return resultfinal;
                }

                var requesterRoles = await _userManager.GetRolesAsync(requester);
                if (!requesterRoles.Contains(AppRoles_Desktop.DepartmentManager))
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }


                Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
                if (managerid == null)
                {
                    resultfinal.Error = "The Requester is not allowed for calling this service";
                    resultfinal.Success = false;
                    return resultfinal;
                }
                bool cc = false;
                string performanceRatingName = "Unknown";
                switch (request.Ratting)
                {
                    case -1:
                        performanceRatingName = "Bad";
                        break;
                    case 0:
                        performanceRatingName = "Default";
                        break;
                    case 1:
                        performanceRatingName = "Excellent";
                        break;
                }
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                  
                    var hasPerformance = await _dbcontext.PerformanceRecorderTable.Where(c => c.EmployeeDepartment_FK == request.EmployeeID).FirstOrDefaultAsync();
                    if (hasPerformance != null)
                    {
                        if(hasPerformance.ReviewerComment==request.Comment && hasPerformance.PerformanceRating == request.Ratting)
                        {
                            resultfinal.Error = "You have already submitted this performance record.";
                            resultfinal.Success = false;
                            return resultfinal;
                        }
                        hasPerformance.PerformanceRating = request.Ratting;
                        hasPerformance.ReviewerComment = request.Comment;
                        cc = false;
                        _dbcontext.Update(hasPerformance);
                        var employee = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Id == hasPerformance.EmployeeDepartment_FK)
                   .Select(c => new
                   {
                       ID = c.EmployeeNA.IdentityUserNA.Id,
                       EmployeeID = c.EmployeeNA.Id,
                       FullName = c.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**No Name**",

                   }
                   ).FirstOrDefaultAsync();

                        var log = new Audit_Logs
                        {
                            ActionType = "Updating",
                            Notes = $"setting the performance record of the employee '{employee.FullName}' to '{performanceRatingName}'",
                            PerformerAccount_FK = managerid,
                            TargetEntity_FK = employee.EmployeeID
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                    }
                    else
                    {
                        cc = true;
                        var performance = new Performance_Recorder
                        {
                            EmployeeDepartment_FK = request.EmployeeID,
                            Reviewer_FK = (Guid)managerid,
                            
                            PerformanceRating = request.Ratting,
                            ReviewerComment = request.Comment,
                           
                            
                        };
                       await _dbcontext.AddAsync(performance);
                        var employee = await _dbcontext.EmployeeDepartmentsTable.Where(c => c.Id == hasPerformance.EmployeeDepartment_FK)
                .Select(c => new
                {
                    ID = c.EmployeeNA.IdentityUserNA.Id,
                    EmployeeID = c.EmployeeNA.Id,
                    FullName = c.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName ?? "**No Name**",

                }
                ).FirstOrDefaultAsync();

                        var log = new Audit_Logs
                        {
                            ActionType = "Updating",
                            Notes = $"setting the performance record of the employee '{employee.FullName}' to '{performanceRatingName}'",
                            PerformerAccount_FK = managerid,
                            TargetEntity_FK = employee.EmployeeID
                        };
                        await _dbcontext.AuditLogsTable.AddAsync(log);
                    }
                    
                  
                
                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
                resultfinal.Success = true;
                resultfinal.Messagee = cc ? "The performance record has been added successfully." : "The performance record has been updated successfully.";
                return resultfinal;

            }
            catch
            {
                resultfinal.Error = "An error occurred while processing your request. Please try again later.";
                resultfinal.Success = false;
                return resultfinal;
            }
        }

    }
}
//