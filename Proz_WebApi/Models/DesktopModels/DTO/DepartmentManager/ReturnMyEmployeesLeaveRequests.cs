namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class ReturnMyEmployeesLeaveRequests
    {
        public Guid LeaveRequestID { get; set; }
        public string employeeName { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
        public string Reason { get; set; }
    }
}
