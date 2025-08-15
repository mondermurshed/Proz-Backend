namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class RetrunFeedbacksInformation
    {
        public Guid FeedbackId { get; set; }
        public string FeedbackType { get; set; }
        public string FeedbackTitle { get; set; }
        public string FeedbackDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FeedbackAnswer { get; set; }
        public DateTime? AnsweredIn { get; set; }
        public string AnsweredBy { get; set; }
      

    }
}
