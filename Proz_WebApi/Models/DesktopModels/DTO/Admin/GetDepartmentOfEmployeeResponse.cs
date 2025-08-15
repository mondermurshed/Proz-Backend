namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class GetDepartmentOfEmployeeResponse
    {
        public bool GotDepartment { get; set; } = false;
        public string DepartmentName { get; set; } = null;
        public Guid DepartmentID { get; set; } = Guid.Empty;

    }
}
