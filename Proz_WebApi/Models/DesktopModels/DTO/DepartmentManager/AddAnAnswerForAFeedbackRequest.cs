namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class AddAnAnswerForAFeedbackRequest
    {
        public Guid TargetedFeedback { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
