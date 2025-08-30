namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class NotificationResponse
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Created_At { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
    }
}
