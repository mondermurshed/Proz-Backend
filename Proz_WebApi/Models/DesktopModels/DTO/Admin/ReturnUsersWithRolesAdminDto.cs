namespace Proz_WebApi.Models.DesktopModels.Dto.Admin
{
    public class ReturnUsersWithRolesAdminDto
    {
        public Guid id { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public bool IsUser { get; set; } = false;
        public bool IsEmployee { get; set; } = false;
        public bool IsDepartmentManager { get; set; } = false;
        public bool IsHRManager { get; set; } = false;
        public bool IsAdmin { get; set; } = false;


    }


}
