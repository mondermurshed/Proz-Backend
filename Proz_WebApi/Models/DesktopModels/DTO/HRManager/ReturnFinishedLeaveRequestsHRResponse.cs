namespace Proz_WebApi.Models.DesktopModels.DTO.HRManager
{
    public class ReturnFinishedLeaveRequestsHRResponse
    {
        public Guid LeaveRequestID { get; set; }
        public string employeeName { get; set; }
    
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string Reason { get; set; }
        public string MyAnswer { get; set; }
        public DateTime LeaveRequestOLD { get; set; }
        public DateTime? AnswerOld { get; set; }
        public DateTime? DecisionOld { get; set; }
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
        public bool Accepted { get; set; }
        public bool? NeedToAgreeOn { get; set; } 
        public bool? AgreedOn { get; set; }
    }
}
