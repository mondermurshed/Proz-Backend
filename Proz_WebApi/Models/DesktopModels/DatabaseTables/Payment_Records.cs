using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Payment_Records
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
    public Guid Id { get; set; } 
    public DateOnly PaymentDateCreatedAt {  get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly PaymentPeriodStart {  get; set; }
    public DateOnly PaymentPeriodEnd { get; set; }
    public int PaymentCounter {  get; set; }
    [Precision(18, 2)]
    public double Salary {  get; set; }
    [MaxLength(4)]
    [Unicode]
    [Required]
    public string SalaryCurrencyType {  get; set; }
    [Precision(18, 2)]
    public double FixedBonus {  get; set; }
    [MaxLength(4)]
    [Unicode]
    [Required]
    public string FixedBonusCurrencyType {  get; set; }
    [StringLength(150, MinimumLength = 25)]
    [Unicode]
    public string? FixedBonusNote { get; set; }
    [Precision(18, 2)]
    public double PerformanceBonus {  get; set; }
    [MaxLength(4)]
    [Unicode]
    [Required]
    public string PerformanceBonusCurrencyType {  get; set; }
    [StringLength(150, MinimumLength = 25)]
    [Unicode]
    public string? PerformanceBonusNote {  get; set; }
    [Precision(18, 2)]
    public double Deduction {  get; set; }
    [MaxLength(4)]
    [Unicode]
    [Required]
    public string DeductionCurrencyType {  get; set; }
    [StringLength(150, MinimumLength = 25)]
    [Unicode]
    public string? DeductionNote {  get; set; }
        public bool Status { get; set; } = true;
    public Guid EmployeeDepartments_FK {  get; set; }
    public Employee_Departments EmployeeDepartmentsNA { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
    }
}
