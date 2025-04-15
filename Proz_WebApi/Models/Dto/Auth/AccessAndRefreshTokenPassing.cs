namespace Proz_WebApi.Models.Dto.Auth
{
    public class AccessAndRefreshTokenPassing
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
