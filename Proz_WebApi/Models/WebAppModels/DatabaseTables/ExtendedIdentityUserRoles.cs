using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Models.WebAppModels.DatabaseTables
{
    public class ExtendedIdentityUserRolesWebApp : IdentityUserRole<string>
    {

        public virtual ExtendedIdentityUsersWebApp UserNA { get; set; }             // Navigation to User  BTW virtual means that it will not create an actual column but rather a column that EF needs to get the data.
        public virtual IdentityRole RoleNA { get; set; }                       // Navigation to Role
    }
}
