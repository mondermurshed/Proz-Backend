using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class ExtendedIdentityRolesDesktop : IdentityRole <Guid>
    {
       
        [MaxLength(20)]
        [Required]
        public string RoleColorCode {  get; set; }

        public virtual ICollection<ExtendedIdentityUserRolesDesktop> UserRolesNA { get; set; } = new List<ExtendedIdentityUserRolesDesktop>();
    }
}
