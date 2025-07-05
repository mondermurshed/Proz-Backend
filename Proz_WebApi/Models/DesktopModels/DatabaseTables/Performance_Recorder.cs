using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Performance_Recorder
    {
     [Key]
     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
     public int Id { get; set; }
     public int PerformanceRating { get; set; } = 0; //-1 means bad, 0 is the default, 1 is excellent
     [MaxLength(125)]
     [Unicode]
     public string? ReviewerComment { get; set; }
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public Guid EmployeeDepartment_FK { get; set; }
     public Employee_Departments EmployeeDepartmentNA { get; set; }
     public Guid Reviewer_FK {  get; set; }
     public Employees ReviewerNA { get; set; }


    }
}
