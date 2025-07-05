using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class LoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      
        public Guid ID { get; set; } 
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        [StringLength(39, MinimumLength = 15)]
        [Unicode]
        public string? IpAddress { get; set; }
        public Guid ExtendedIdentityUsersDesktop_FK { get; set; } 
        public ExtendedIdentityUsersDesktop ExtendedIdentityUsersDesktopNA { get; set; }
    }
}
