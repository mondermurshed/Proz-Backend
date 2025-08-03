namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class DepartmentCreatingRequest
    {
    public Guid ManagerID { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    }
}
