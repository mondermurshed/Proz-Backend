using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Health_Information
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(500, MinimumLength = 3)]
        [Unicode]
        public string? MedicalConditions { get; set; }
        [StringLength(500, MinimumLength = 3)]
        [Unicode]
        public string? Allergies {  get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string EmergencyContactName { get; set; }
        [MaxLength(15)]
        [Unicode]
        [Required]
        public string EmergencyContactPhone { get; set; }
        [StringLength(5, MinimumLength = 2)]
        [Unicode]
        [Required]
        public string CountryCodeOfThePhone { get; set; }
        public Guid PersonalInformation_FK {  get; set; }
        public Personal_Information PersonalInformationNA { get; set; }
    }
}
