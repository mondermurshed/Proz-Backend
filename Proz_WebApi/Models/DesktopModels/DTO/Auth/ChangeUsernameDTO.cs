namespace Proz_WebApi.Models.DesktopModels.DTO.Auth
{
    public class ChangeUsernameDTO
    {
        public string? CurrentPassword { get; set; } = null;
        public string? NewUsername { get; set; } = null;
    }
}
