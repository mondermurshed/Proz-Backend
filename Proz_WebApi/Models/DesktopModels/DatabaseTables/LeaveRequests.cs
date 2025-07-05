using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class LeaveRequests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
     public Guid Id { get; set; }
    [StringLength(35, MinimumLength = 3)]
    [Unicode]
    [Required]
   
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [StringLength(500, MinimumLength = 50)]
    [Unicode]
    [Required]
     public string Reason { get; set; }
     public bool IsEditable { get; set; } = true; //the user can edit or fix his text ? it will not be editable if a department manager or hr manager/admin reply to pin his request

    public DateTime LastUpdate {  get; set; } = DateTime.UtcNow;
    public bool? HasSanctions {  get; set; }
    public bool? AgreedOn { get; set; }

    [MaxLength(15)]
    [Unicode]
    public string? DMStatus {  get; set; }
    [MaxLength(15)]
    [Unicode]
    public string? FinalStatus {  get; set; }
    public DateTime Created_At {  get; set; } = DateTime.UtcNow;
    [MaxLength(250)]
    [Unicode]
    public string? DepartmentManagerComment {  get; set; }
    [MaxLength(250)]
    [Unicode]
    public string? FinalStatus_Comment {  get; set; }
    public DateTime? Decision_At {  get; set; }
    public Guid Requester_Employee_FK { get; set; }
    public Employees EmployeeNA { get; set; }
    public Guid? DepartmentManager_FK {  get; set; } 
    public Employees DepartmentManagerNA { get; set; }
    public Guid? HandlerEmployee_FK { get; set; }
    public Employees HandlerEmployeeNA { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
    }
}
