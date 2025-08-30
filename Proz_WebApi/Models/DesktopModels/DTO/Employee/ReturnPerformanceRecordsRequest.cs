namespace Proz_WebApi.Models.DesktopModels.DTO.Employee
{
    public class ReturnPerformanceRecordsListRequest
    {
        public int Month { get; set; }
    }
    public class ReturnPerformanceRecordsResponse
    {
        public int PerformanceRating { get; set; }
        public string? ReviewerComment { get; set; }
        public DateOnly CreatedAt { get; set; } 
        public string Reviewer { get; set; }
    }
}
