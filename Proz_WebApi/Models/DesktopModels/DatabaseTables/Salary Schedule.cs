using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Salary_Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
        public Guid Id { get; set; }
        public DateOnly CurrentPeriodStartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly CurrentPeriodEndDate { get; set; }
        public int PaymentCounter { get; set; }
        public Double EmployeeBonus { get; set; }
        public Double Employee_Deduction { get; set; }
        public Guid EmployeeDepartment_FK { get; set; }
        public Employee_Departments EmployeeDepartmentNA { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
   
}
