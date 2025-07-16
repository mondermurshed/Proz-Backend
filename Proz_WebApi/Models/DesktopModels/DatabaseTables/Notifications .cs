using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Notifications
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {  get; set; }
    [StringLength(50, MinimumLength = 5)]
    [Unicode]
    [Required]
    public string Title { get; set; }
    [StringLength(500, MinimumLength = 25)]
    [Unicode]
    [Required]
    public string Message { get; set; }
    public DateTime Created_At { get; set; } = DateTime.UtcNow;
    public DateTime? Seen_At { get; set; }
    [MaxLength(25)]
    [Unicode]
    [Required]
    public string? Type {  get; set; }
    [MaxLength(6)]
    [Unicode]
    [Required]
    public string Priority {  get; set; }
    [Required]
    public bool IsArchived { get; set; } = false; //meaning it's not Archived yet (the default when the record is just created, the receiver then can set it to true meaning Archived  (soft-deleted, hidden from main views, but kept for historical/backup purposes).
    public Guid Target_FK {  get; set; }
    public Employees EmployeeNA { get; set; }
    }
}
