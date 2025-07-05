namespace Proz_WebApi.Models.DesktopModels.DTO.HRManager
{

    public class UserInformationClass
    {

        public Guid UserId { get; set; }
        public List<string> NewRoles { get; set; }
        public bool ReplaceExisting { get; set; } = true;
    }

    public class RoleUpdateDTO
    {
        // A collection of per-user role change instructions.
        public List<UserInformationClass> UserInformation { get; set; }
    }
}
