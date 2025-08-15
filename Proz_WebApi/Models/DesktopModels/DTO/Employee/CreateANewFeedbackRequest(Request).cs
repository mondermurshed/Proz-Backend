namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class CreateANewFeedbackRequest_Request
    {
        public string FeedbackTitle { get; set; }

        public string FeedbackDescription { get; set; }

        public Guid FeedbackType { get; set; }
    }
}
