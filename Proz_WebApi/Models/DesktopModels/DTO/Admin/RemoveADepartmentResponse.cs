namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class RemoveADepartmentResponse
    {
        public List<string>? Message { get; set; } = null;
        public List<string>? Errors { get; set; } = null;
        public bool NeedesAdminApproval { get; set; } = false;
    }
}
