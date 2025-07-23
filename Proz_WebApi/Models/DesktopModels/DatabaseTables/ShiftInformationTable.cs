using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class ShiftInformationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id {  get; set; }
        public TimeOnly Shift_Starts { get; set; }
        public TimeOnly Shift_Ends { get; set; }
        public int? TotalHours { get; set; }
        [Required]
        [MaxLength(15)]
        [Unicode]
        public string ShiftType { get; set; }

        public Guid Department_FK { get; set; }
        public Departments DepartmentNA { get; set; }
        public ICollection<Breaks_Time> BreaksTimeNA { get; set; }

    }
}
