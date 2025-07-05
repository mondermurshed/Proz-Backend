namespace Proz_WebApi.Models.DesktopModels.Dto.Auth
{
    public class UserRegisterationTemp
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // This can be plain for now, or pre-hashed using a hasher
    }
}
