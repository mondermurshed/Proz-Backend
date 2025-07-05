namespace Proz_WebApi.Models.DesktopModels.DTO.LoginHistoryDTOs
{
    public class LoginHistoryDto
    {
        public DateTime LoggedAt { get; set; }
        public string? IpAddress { get; set; }
    }
}
