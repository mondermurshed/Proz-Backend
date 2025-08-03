namespace Proz_WebApi.Models.DesktopModels.DTO.Auth
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
        public string DeviceToken { get; set; }
    }
}
