namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class AgreeOnLeaveRequestDecisionRequest
    {
        public Guid LeaveRequestID {  get; set; }
        public bool Agreed { get; set; }
    }
}
