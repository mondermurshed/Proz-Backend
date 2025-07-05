using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class ExtendedIdentityUserRolesDesktop : IdentityUserRole<Guid>
    {

        public virtual ExtendedIdentityUsersDesktop UserNA { get; set; }             // Navigation to User  BTW virtual means that it will not create an actual column but rather a column that EF needs to get the data.
        public virtual ExtendedIdentityRolesDesktop RoleNA { get; set; }                       // Navigation to Role
    }
}
