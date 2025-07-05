using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class EmployeeSalaryHistory
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Precision(18, 2)]
    public double Salary {  get; set; }
    [MaxLength(3)]
    [Unicode]
    [Required]
    public string CurrencyType {  get; set; }
    [MaxLength(3)]
    [Unicode]
    [Required]
    public string PaymentFrequency { get; set; } 
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);   
        public DateOnly? EffectiveTo { get; set; } = null; //it is nullable because if it was null then it means that there isys  no other salary was changed for a department the whole 30 days or the whole 7 days frequency was set Weekly.
    public Guid EmployeeDepartments_FK {  get; set; }
    public Employee_Departments EmployeeDepartmentsNA { get; set; }

    }
}
