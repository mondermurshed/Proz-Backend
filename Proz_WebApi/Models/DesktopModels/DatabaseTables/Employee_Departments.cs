using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Employee_Departments
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Precision(18, 2)]
        public Double Salary { get; set; }
        [MaxLength(4)]
        [Unicode]
        [Required]
        public string Salary_Currency_Type { get; set; }
        [Precision(18, 2)]
        public double? Company_Bonus { get; set; }
        [MaxLength(4)]
        [Unicode]
        public string? Company_Bonus_Currency_Type { get; set; }
        [MaxLength(7)]
        [Unicode]
        [Required]
        public string Payment_Frequency {  get; set; }
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); 
        public Guid Employee_FK { get; set; }
        public Employees EmployeeNA { get; set; }
        public int Department_FK { get; set; }
        public Departments DepartmentNA { get; set; }
        public int Shift_FK {  get; set; }
        public ShiftInformationTable ShiftNA { get; set; }    
        public ICollection<Audit_Logs> Audit_LogsNA { get; set; } = new List<Audit_Logs>();
        public Salary_Schedule SalaryScheduleNA { get;  set; }
        public ICollection<Payment_Records> PaymentRecordsNA { get; set; } = new List<Payment_Records>();
        public ICollection<Attendance_Recorder> AttendanceRecordersNA { get;set; } = new List<Attendance_Recorder>();
        public ICollection<Performance_Recorder> PerformanceRecordersNA { get; set; } = new List<Performance_Recorder>();
        public ICollection<EmployeeSalaryHistory> EmployeeSalaryHistoryNA { get; set; } = new List<EmployeeSalaryHistory>();
 
    }
}
