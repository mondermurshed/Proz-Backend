namespace Proz_WebApi.Models.Dto.Admin
{
    public class ReturnUsersWithRolesAdminDto
    {
        public string id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
