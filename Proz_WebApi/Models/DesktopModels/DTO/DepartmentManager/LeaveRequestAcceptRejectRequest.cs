namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class LeaveRequestAcceptRejectRequest
    {
        public Guid LeaveRequestID {  get; set; }
        public bool Accept { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
