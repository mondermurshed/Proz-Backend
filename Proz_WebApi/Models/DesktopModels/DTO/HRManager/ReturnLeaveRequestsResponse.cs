namespace Proz_WebApi.Models.DesktopModels.DTO.HRManager
{
    public class ReturnLeaveRequestsResponse
    {
        public Guid LeaveRequestID { get; set; }
        public string employeeName { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string Reason { get; set; }
        public string DMName { get; set; }
        public string Department { get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
