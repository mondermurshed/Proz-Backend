using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Feedback_Types
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [StringLength(20, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string FeedbackType { get; set; }

        public ICollection<Feedbacks> FeedbacksNA { get; set; } = new List<Feedbacks>();
    }
}
