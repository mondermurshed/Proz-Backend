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
    

    public DateTime LastUpdate {  get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdateDM { get; set; } 
    public DateTime? LastUpdateHR { get; set; } 
        public bool? HasSanctions {  get; set; }
    public bool? AgreedOn { get; set; }
    public bool? Completed { get; set; } = false;
        [MaxLength(15)]
        [Unicode]
     public string RequesterStatus { get; set; } = "Waiting";
     [MaxLength(15)]
    [Unicode]
    public string? DMStatus {  get; set; }
    [MaxLength(15)]
    [Unicode]
    public string? FinalStatus {  get; set; }
   
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
