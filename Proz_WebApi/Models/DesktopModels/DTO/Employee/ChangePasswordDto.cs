namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
