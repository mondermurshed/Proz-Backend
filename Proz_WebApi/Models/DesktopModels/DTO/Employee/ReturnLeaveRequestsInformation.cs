namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class ReturnLeaveRequestsInformation
    {
        public Guid LeaveRequestId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string Reason { get; set; }
        public string DepartmentManagerComment { get; set; } 
        public string FinalStatusComment { get; set; }

        public string RequesterStatus { get; set; } = "Waiting";
        public string DMStatus { get; set; } = "Waiting";
        public string FinalStatus { get; set; } = "Waiting";

       
        public DateTime SentAt { get; set; }
        public DateTime? DMAnsweredAt { get; set; }
        public DateTime? HRMAnsweredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool? HasSanctions { get; set; }
        public bool? AgreedOn { get; set; }
        public bool? Completed { get; set; } = false;
        
        public string DMName { get; set; }
        public string HRMName { get; set; }
    }
}
