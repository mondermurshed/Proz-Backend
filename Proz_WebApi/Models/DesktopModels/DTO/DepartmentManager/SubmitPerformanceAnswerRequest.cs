namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class SubmitPerformanceAnswerRequest
    {
        public Guid EmployeeID { get; set; }
        public int Ratting {  get; set; }
        public string Comment { get; set; }
    }
}
