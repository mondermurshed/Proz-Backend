using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Personal_Information
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        [MaxLength(150)]
        [Unicode]
        [Required]
        public string FullName { get; set; }
        public int Age {  get; set; }
        public DateOnly DateOfBirth { get; set; }
        [StringLength(6, MinimumLength = 4)]
        [Unicode]
        [Required]
        public string Gender { get; set; }
        [MaxLength(50)]
        [Unicode]
        public string? Nationality {  get; set; }
        public bool LivingOnPrimaryPlace {  get; set; }
        public Guid IdentityUser_FK {  get; set; }
        public ExtendedIdentityUsersDesktop IdentityUserNA {  get; set; }
        public CurrentAddress CurrentAddressNA { get; set; }
        public Health_Information HealthInformationNA { get; set; }
        public ICollection<Personal_Phone_Numbers> PhoneNumbersNA { get; set; } = new List<Personal_Phone_Numbers>();



    }
}
