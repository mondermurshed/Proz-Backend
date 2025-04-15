namespace Proz_WebApi.Models.Dto.DepartmentManager
{
    public class RoleChangeRequestDM
    {
     
            public List<string> UsersIDs { get; set; } =new List<string>();
            public string NewRole;
       
        
    }
}
