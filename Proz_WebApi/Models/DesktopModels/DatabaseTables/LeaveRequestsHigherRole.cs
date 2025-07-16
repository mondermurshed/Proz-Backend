using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class LeaveRequestsHigherRole
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


        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdateAdmin { get; set; }
        public bool? HasSanctions { get; set; }
        public bool? AgreedOn { get; set; }
        public bool? Completed { get; set; } = false;
        [MaxLength(15)]
        [Unicode]
        public string RequesterStatus { get; set; } = "Waiting";
       
    
        [MaxLength(15)]
        [Unicode]
        public string? HRStatus { get; set; }

     
        [MaxLength(250)]
        [Unicode]
        public string? HR_Comment { get; set; }
        public DateTime? Decision_At { get; set; }
        public Guid Requester_Employee_FK { get; set; }
        public Employees RequesterManagerNA { get; set; }
        public Guid? HREmployee_FK { get; set; }
        public Employees HRManagerNA { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
