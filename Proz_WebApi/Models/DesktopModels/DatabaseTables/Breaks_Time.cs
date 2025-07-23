using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Breaks_Time
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    public TimeOnly BreakStart {  get; set; }
    public TimeOnly BreakEnd { get; set; }
    [StringLength(35, MinimumLength = 3)]
    [Unicode]
    [Required]
    public string BreakType { get; set; }
    [StringLength(350, MinimumLength = 3)]
    [Unicode]
    public string? Notes {  get; set; }
    public Guid Shift_FK {  get; set; }
    public ShiftInformationTable ShiftNA { get; set; }
    }
}
