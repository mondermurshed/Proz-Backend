namespace Proz_WebApi.Models.Dto.Admin
{

    public class UserInformation 
    {
    
        public string UserId { get; set; }
        public List<string> NewRoles { get; set; }
        public bool ReplaceExisting { get; set; } = true;
    }

    public class RoleChangeRequestAdmin
    {
        // A collection of per-user role change instructions.
        public List<UserInformation> UserInformation { get; set; }
    }
}
