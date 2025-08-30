using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Data;
using Proz_WebApi.Helpers_Types;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;
using Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager;
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


        public async Task<IEnumerable<ReturnLeaveRequestsResponse>> ReturnLeaveRequests(string requesterID)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.HRManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var LeaveRequests = await _dbcontext.LeaveRequestsTable.Where(c => c.DepartmentManager_FK != null && c.Completed==false && c.DMStatus=="Accepted" && c.EmployeeNA.Department_FK!=null && c.HandlerEmployee_FK==null).Select(ur => new ReturnLeaveRequestsResponse
            {
                LeaveRequestID = ur.Id,
                employeeName = ur.EmployeeNA.EmployeeNA.IdentityUserNA.PersonalInformationNA.FullName,
                From = ur.StartDate,
                To = ur.EndDate,
                Reason = ur.Reason,
                Department = ur.EmployeeNA.DepartmentNA.DepartmentName,
                DMName = ur.DepartmentManagerNA.IdentityUserNA.PersonalInformationNA.FullName,
                CreatedAt=ur.LastUpdate
            }).ToListAsync();

            if (LeaveRequests == null || LeaveRequests.Count == 0)
                return null;
            else
                return LeaveRequests;

        }


        public async Task<AddAnAnswerForALeaveRequestHRMResponse> CreateAnAnswerForALeaveRequest(string requesterID, LeaveRequestAcceptRejectHRMRequest request)
        {
            var resultfinal = new AddAnAnswerForALeaveRequestHRMResponse();
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
                if (!requesterRoles.Contains(AppRoles_Desktop.HRManager))
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
                    if (LeaveRequest.LastUpdateHR != null)
                    {
                        resultfinal.Error = "This Leave Request is already answered.";
                        resultfinal.Success = false;
                        return resultfinal;
                    }
                    LeaveRequest.HandlerEmployee_FK = managerid;
                    LeaveRequest.FinalStatus_Comment = request.Comment;
                    LeaveRequest.LastUpdateHR = DateTime.UtcNow;
                 
                    LeaveRequest.FinalStatus = "Accepted";

                    if (request.Accept == false)
                    {
                        LeaveRequest.FinalStatus = "Rejected";
                        LeaveRequest.RequesterStatus = "Done";
                        LeaveRequest.Completed = true;                                   
                        LeaveRequest.Decision_At = DateTime.UtcNow;

                    }
                    if(request.MustAgreeOn == true && request.Accept == true)
                    {
                        LeaveRequest.HasSanctions = true;
                        LeaveRequest.RequesterStatus = "Waiting";
                        LeaveRequest.Completed = false;
                        LeaveRequest.Decision_At = null;
                    }
                    else
                    {
                        LeaveRequest.HasSanctions = false;
                        LeaveRequest.RequesterStatus = "Done";
                        LeaveRequest.Completed = true;
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
                    if (request.Accept==false)
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
                    else
                    {
                    if(request.MustAgreeOn==true)
                        {
                            var log = new Audit_Logs
                            {
                                ActionType = "Creating",
                                Notes = $"Accepting a leave request of the employee '{employee.FullName}' with some sanctions",
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
                                Notes = $"Accepting a leave request of the employee '{employee.FullName}'",
                                PerformerAccount_FK = managerid,
                                TargetEntity_FK = employee.EmployeeID
                            };
                            await _dbcontext.AuditLogsTable.AddAsync(log);
                        }
                    }
                   

                    await _dbcontext.SaveChangesAsync();
                    scope.Complete();
                }
                if (request.Accept == true)
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


        public async Task<IEnumerable<ReturnFinishedLeaveRequestsHRResponse>> ReturnFinishedLeaveRequestsOfADepartment(string requesterID)
        {
            var requester = await _userManager.FindByIdAsync(requesterID);
            if (requester == null)
                return null;

            var requesterRoles = await _userManager.GetRolesAsync(requester);
            if (!requesterRoles.Contains(AppRoles_Desktop.HRManager))
                return null;


            Guid? managerid = await _dbcontext.EmployeesTable.Where(c => c.IdentityUsers_FK == requester.Id).Select(c => c.Id).FirstOrDefaultAsync();
            if (managerid == null)
                return null;

            var LeaveRequests = await _dbcontext.LeaveRequestsTable.Where(x => x.HandlerEmployee_FK == managerid && x.Completed == true).Select(x => new ReturnFinishedLeaveRequestsHRResponse
            {


                LeaveRequestID = x.Id,
                Reason = x.Reason,
                MyAnswer = x.FinalStatus_Comment ?? "**No Answer**",
                From = x.StartDate,
                To = x.EndDate,

                LeaveRequestOLD = x.LastUpdate,
                AnswerOld = x.LastUpdateHR,

                ManagerName = x.DepartmentManagerNA != null &&
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

                Accepted = x.FinalStatus == "Accepted" ? true : false,
                DepartmentName = x.EmployeeNA != null && x.EmployeeNA.DepartmentNA != null &&
                !string.IsNullOrEmpty(x.EmployeeNA.DepartmentNA.DepartmentName) ? x.EmployeeNA.DepartmentNA.DepartmentName : "**Not assigned to a department**",

                NeedToAgreeOn = x.HasSanctions,

                AgreedOn=x.AgreedOn ?? false,
                
                DecisionOld=x.Decision_At


            }).ToListAsync();

            if (LeaveRequests == null || LeaveRequests.Count == 0)
                return null;
            else
                return LeaveRequests;

        }







    }
}
