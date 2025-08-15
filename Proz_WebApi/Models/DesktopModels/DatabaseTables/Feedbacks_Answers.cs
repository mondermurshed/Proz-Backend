using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Feedbacks_Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(1500, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string Answer { get; set; }
         public DateTime? LastUpdated {  get; set; } = DateTime.UtcNow;
        public Guid Feedback_FK { get; set; }
       
       public Feedbacks FeedbackNA { get; set; }

       public Guid RespondentAccount_FK { get; set; }
       public Employees EmployeeNA { get; set; }
    }
}
