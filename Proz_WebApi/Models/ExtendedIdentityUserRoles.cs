using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Models
{
    public class ExtendedIdentityUserRoles : IdentityUserRole<String>
    {
        
        public virtual ExtendedIdentityUsers User { get; set; }             // Navigation to User  BTW virtual means that it will not create an actual column but rather a column that EF needs to get the data.
        public virtual IdentityRole Role { get; set; }                       // Navigation to Role
    }
}
