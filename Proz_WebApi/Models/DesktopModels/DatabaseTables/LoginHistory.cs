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
        public string? DeviceTokenhashed { get; set; }
        public string? DeviceName { get; set; }
        public Guid ExtendedIdentityUsersDesktop_FK { get; set; } 
        public ExtendedIdentityUsersDesktop ExtendedIdentityUsersDesktopNA { get; set; }
    }
}
