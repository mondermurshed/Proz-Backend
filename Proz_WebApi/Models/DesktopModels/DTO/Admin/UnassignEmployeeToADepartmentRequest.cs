namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class UnassignEmployeeToADepartmentRequest
    {
        public Guid EmployeeID { get; set; }
        public Guid DepartmentID { get; set; }
    }
}
