namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class ReturnSystemRoles
    {
        public string RoleName { get; set; } = string.Empty;
        public string RoleColorCode { get; set; } = string.Empty;
        public Guid RoleID { get; set; }
    }
}
