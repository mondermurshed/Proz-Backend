using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class ExtendedIdentityUsersDesktop : IdentityUser <Guid> //always when you want to extend a pre defined table like identityuser from the package then first create a DTO like this and make it inhert from the one that you want to replace it with.
    {
        //put here any column that you want to add to the identity user table in the database, this class will act as an extended to it.
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateTime LastOnline {  get; set; } = DateTime.UtcNow;
        [StringLength(15, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string Status { get; set; } = "Pending";
        public virtual ICollection<ExtendedIdentityUserRolesDesktop> UserRolesNA { get; set; } = new List<ExtendedIdentityUserRolesDesktop>(); //virtual means that this is not a real column that will be created inside the database but rather then that a navigation property
        public ICollection<RefreshTokenDesktop> RefreshTokensNA { get; set; } = new List<RefreshTokenDesktop>();
        public Personal_Information PersonalInformationNA { get; set; }
        public ICollection<LoginHistory> LoginHistroyNA { get; set; } = new List<LoginHistory>();
        public Employees EmployeesNA { get; set; }
    }
}
