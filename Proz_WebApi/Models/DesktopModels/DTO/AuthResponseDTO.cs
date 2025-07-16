namespace Proz_WebApi.Models.DesktopModels.DTO
{
    public class AuthResponseDTO
    {
        public AuthResponseDTO ()
        {
            Message = new List<string>();
            Error = new List<string>();
        }
        public List<string> Message { get; set; }
        public List<string> Error { get; set; }
    }
}
