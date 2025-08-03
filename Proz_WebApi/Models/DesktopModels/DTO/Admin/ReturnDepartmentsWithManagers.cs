namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class ReturnDepartmentsWithManagers
    {
      
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
        public Guid DepartmentID { get; set; }
        public Guid ManagerID { get; set; }



    }
}
