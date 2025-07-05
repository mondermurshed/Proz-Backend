using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Models.WebAppModels.DatabaseTables
{
    public class ExtendedIdentityUsersWebApp : IdentityUser //always when you want to extend a pre defined table like identityuser from the package then first create a DTO like this and make it inhert from the one that you want to replace it with.
    {
        //put here any column that you want to add to the identity user table in the database, this class will act as an extended to it.
        public virtual ICollection<ExtendedIdentityUserRolesWebApp> UserRolesNA { get; set; } = new List<ExtendedIdentityUserRolesWebApp>(); //virtual means that this is not a real column that will be created inside the database but rather then that a navigation property
    }
}
