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
        public Money? EmployeeBonus { get; set; }
        public Money? Employee_Deduction { get; set; }
        public Guid EmployeeDepartment_FK { get; set; }
        public Employee_Departments EmployeeDepartmentNA { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
    public class Money
    {
        [Precision(18, 2)]
        public double Amount { get; set; }  /*and inside the main model he will have to put public Money EmployeeBonus { get; set; } and   public Money Employee_Deduction { get; set; } and make these pairs null or not null (as you want). EF will name the columns inside the database as the following [Parent Property Name] + "_" + [Value Object Field Name] so EmployeeBonus_Amount or Employee_Deduction_Currency.*/
        [MaxLength(4)]
        [Unicode]
        [Required]
        public string Currency { get; set; } /*Finally we need to put something inside Fluent API. GPT4 will tell you if you need it later.*/
    }
}
