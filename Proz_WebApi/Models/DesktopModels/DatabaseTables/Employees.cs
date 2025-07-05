using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Employees
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      
        public Guid Id { get; set; }
        public DateOnly HireDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [MaxLength(15)]
        [Unicode]
        [Required]
        public string Status { get; set; } = "Active"; //(Active, Suspended)
        public Guid IdentityUsers_FK { get; set; }
        public ExtendedIdentityUsersDesktop IdentityUserNA { get; set; }
        public ICollection<Feedbacks> FeedbacksNA {  get; set; } = new List<Feedbacks>();
        public ICollection<Feedbacks_Answers> FeedbackAnswerNA { get; set; } = new List<Feedbacks_Answers>();
        public ICollection<Departments> ManagerAtNA { get; set; } = new List<Departments>();
        public ICollection <Audit_Logs> AuditLogsNA { get; set; } = new List<Audit_Logs>(); 
        public ICollection<Performance_Recorder> PerformanceRecorderNA { get; set; } = new List<Performance_Recorder>();
        public ICollection<LeaveRequests> LeaveRequestsNA { get; set; } = new List<LeaveRequests>();
        public ICollection<LeaveRequests> DepartmentManagerLeaveRequestsDealsNA { get; set; } = new List<LeaveRequests>();
        public ICollection<LeaveRequests> ADHRLeaveRequestsDealsNA { get; set; } = new List<LeaveRequests>();
        public ICollection<Notifications> NotificationsNA { get; set; } = new List<Notifications>();
       
    }
}
