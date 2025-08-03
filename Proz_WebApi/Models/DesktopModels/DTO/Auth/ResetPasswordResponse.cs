namespace Proz_WebApi.Models.DesktopModels.DTO.Auth
{
    public class ResetPasswordResponse
    {
    public List<string>? Messages { get; set; } = new List<string>();
    public List<string>? Errors { get; set; } = new List<string>();
     public bool? IsPasswordProblem { get; set; } = false;
     public int? PasswordScore { get; set; }
    public string? Strength {  get; set; }
    public string? CrackTime { get; set; } = string.Empty;
    public List<string>? Suggestions { get; set; } = new List<string>();
    }
}
