using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Feedbacks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id {  get; set; }
        [StringLength(15, MinimumLength = 3)]
        [Unicode]
        [Required]

        public string FeedbackTitle { get; set; }
        [StringLength(1500, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string FeedbackDescription { get; set;}

        public bool IsSeen { get; set; } = false;
        public bool IsCompleted {  get; set; } = true; //this is for the requester to tell him that this feedback is already got answered by someon and it's completed (No longer can be updated but it can be deleted if the requester wants).
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; //the last time the requester update his feedback, so should know this so we can make his feedback visible when a period of time pass and not directly visible the second he did the update.


        public Guid FeedbackType_FK { get; set; }
        public Feedback_Types FeedbackTypeNA { get; set; }
        public Guid Employee_FK { get; set; }
        public Employees EmployeeNA { get; set; } 
        public Feedbacks_Answers FeedbacksAnswerNA { get; set; }

    }
}
