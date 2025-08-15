namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class ReturnFinishedFeedbacksResponse
    {
        public Guid FeedbackID { get; set; }
        public string FeedbackTitle { get; set; }
        public string FeedbackDescription { get; set; }
        public string MyAnswer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AnsweredAt { get; set; } = DateTime.UtcNow;
        public string FeedbackTypeName { get; set; }
        public string RequesterEmployeeName { get; set; }
    }
}
