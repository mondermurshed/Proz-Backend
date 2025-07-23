using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class GettingStartedTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Unicode]
      
        public string? CompanyName { get; set; }
        [Unicode]
        [Required]
        public string CurrenyType { get; set; } = "USD";
        [Required]
        public DateOnly SystemFirstRun { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Required]
        public Guid Admin_FK { get; set; }
        public ExtendedIdentityUsersDesktop AdminNA { get; set; }

    }
}
