namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class GetLogsForAPersonResponse
    {
        public string ActionType { get; set; }
        public DateTime Performed_At { get; set; }

        public string Notes { get; set; }
        public string Targeted { get; set; }
    }
}
