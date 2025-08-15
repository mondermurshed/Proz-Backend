namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class ReturnFeedbacksResponse
    {
        public Guid FeedbackID { get; set; }
        public string FeedbackTitle { get; set; }
        public string FeedbackDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FeedbackTypeName { get; set; }
        public string RequesterEmployeeName { get; set; }
    }
}
