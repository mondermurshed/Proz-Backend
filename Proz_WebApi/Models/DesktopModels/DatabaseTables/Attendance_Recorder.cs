using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Attendance_Recorder
    { //value types like int, double or timeonly or timedate or GUID are always not null by default so you don't have to put [Required] above them, you only do that for reference types, and to make refrence type + value types to be nullable then add '?' at the end of the property type.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public TimeOnly CheckInTime { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow);

        public TimeOnly? CheckOutTime { get; set; }
    [StringLength(20, MinimumLength = 3)]
    [Unicode]
    [Required]
        public string CheckInStatus {  get; set; }
    [StringLength(20, MinimumLength = 3)]
    [Unicode]
      
        public string? CheckOutStatus { get; set;}
    [StringLength(350, MinimumLength = 3)]
    [Unicode]
       
        public string? CheckInComment {  get; set; }
    [StringLength(350, MinimumLength = 3)]
    [Unicode]
      
    public string? CheckOutComment { get; set; }
    public Guid EmployeeDepartment_FK { get; set; }
    public Employee_Departments EmployeeDepartmentNA { get; set; }
    }
}
