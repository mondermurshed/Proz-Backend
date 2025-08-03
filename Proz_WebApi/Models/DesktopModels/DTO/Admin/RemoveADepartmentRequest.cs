namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class RemoveADepartmentRequest
    {
        public Guid DepartmentID { get; set; }

        public bool WithUnassignEmployeesFromItAgreement { get; set; } = false;
    }
}
