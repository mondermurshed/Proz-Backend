namespace Proz_WebApi.Models.DesktopModels.Dto.Auth
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; } 
        public string DeviceName { get; set; }
    }
}
