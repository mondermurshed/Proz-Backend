using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Audit_Logs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(25, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string ActionType {  get; set; }
        public DateTime Performed_At { get; set; } = DateTime.UtcNow;
        [StringLength(350, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string Notes {  get; set; }
        public Guid PerformerAccount_FK {  get; set; }
        public Guid TargetEntity_FK {  get; set; }
        public Employees PerformerAccountNA { get; set; }
        public Employee_Departments TargetEntityNA { get; set; }
    }
}
