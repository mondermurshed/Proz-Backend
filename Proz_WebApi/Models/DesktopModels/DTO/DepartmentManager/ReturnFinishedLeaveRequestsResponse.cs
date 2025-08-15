namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class ReturnFinishedLeaveRequestsResponse
    {
        public Guid LeaveRequestID { get; set; }
        public string employeeName { get; set; }
        public string SenderName { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string Reason { get; set; }
        public string MyAnswer {  get; set; }
        public DateTime LeaveRequestOLD { get; set; }
        public DateTime? AnswerOld { get; set; }

        public bool Accepted { get; set; }
    }
}
