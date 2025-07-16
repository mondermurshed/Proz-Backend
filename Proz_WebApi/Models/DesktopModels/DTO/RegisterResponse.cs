namespace Proz_WebApi.Models.DesktopModels.DTO
{
    public class RegisterResponse
    {
        public RegisterResponse()
        {
            Suggestions = new List<string>();
            Message = new List<string>();
            Error = new List<string>();
            PasswordCause = false;
        }
        public List<string> Message { get; set; }
        public List<string> Error { get; set; }
        public int Score { get; set; }
        public string Strength { get; set; }
        public string CrackTime { get; set; }
        public List<string> Suggestions { get; set; }
        public bool PasswordCause { get; set; }
    }
}
