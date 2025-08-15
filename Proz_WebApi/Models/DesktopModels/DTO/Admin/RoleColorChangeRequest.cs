namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class RoleColorChangeRequest
    {
        public Guid ID { get; set; }
        public string ColorCode { get; set; } = string.Empty;
    }
}
