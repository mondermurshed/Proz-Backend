namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class CreateANewLeaveRequest_Request
    {
        public string FromDATE { get; set; }
        public string ToDATE {  get; set; }
        public string Reason { get; set; }
    }
}
