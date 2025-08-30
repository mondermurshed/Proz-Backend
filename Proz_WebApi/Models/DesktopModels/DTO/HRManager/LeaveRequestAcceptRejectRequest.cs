namespace Proz_WebApi.Models.DesktopModels.DTO.HRManager
{
    public class LeaveRequestAcceptRejectHRMRequest
    {
        public Guid LeaveRequestID { get; set; }
        public bool Accept { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool MustAgreeOn { get; set; }
    }
}
