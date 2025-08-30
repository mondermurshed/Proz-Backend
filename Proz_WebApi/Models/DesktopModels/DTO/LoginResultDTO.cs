using Org.BouncyCastle.Pkcs;

namespace Proz_WebApi.Models.DesktopModels.DTO
{
    public class LoginResultDTO
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();

        public int ExpiredInSeconds { get; set; }
    }
}
