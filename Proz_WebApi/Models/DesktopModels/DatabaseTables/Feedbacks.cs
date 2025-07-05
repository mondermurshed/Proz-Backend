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
        public DateTime FeedbackDate { get; set; } = DateTime.UtcNow;
        public bool IsEditAble { get; set; } = true; //this to check if the user (the feedback creator) has the ability to update his feedback (true means no one watch it yet)
        public bool CanBeSeen { get; set; } = true; //this is for any one try to reply(answer) the feedback, if it's false then the user is currenly updating something inside the feedback, so wait when he finish it will be true.
        public bool IsCompleted {  get; set; } = true; //this is for the requester to tell him that this feedback is already got answered by someon and it's completed (No longer can be updated but it can be deleted if the requester wants).


        public Guid FeedbackType_FK { get; set; }
        public Feedback_Types FeedbackTypeNA { get; set; }
        public Guid Employee_FK { get; set; }
        public Employees EmployeeNA { get; set; } 
        public Feedbacks_Answers FeedbacksAnswerNA { get; set; }

    }
}
