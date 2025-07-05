namespace Proz_WebApi.Models.DesktopModels.Dto.Admin
{
    public class ReturnUsersWithRolesAdminDto
    {
        public Guid id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
