using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class DepartmentContactMethods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    [StringLength(20, MinimumLength = 3)]
    [Unicode]
    [Required]
    public string ContactMethod { get; set; } //phone or email etc..
    [StringLength(320, MinimumLength = 3)]
    [Unicode]
    [Required]
    public string ContactDetail { get; set; } //the actual information like a phone number or an email address etc..
    [StringLength(20, MinimumLength = 3)]
    [Unicode]
    [Required]
    public string Purpose { get; set; } //is the information used for General Inquiry, Support, Manager Direct or Critical thing only
    public int Department_FK { get; set; }
    public Departments Department { get; set; }
    }
}
