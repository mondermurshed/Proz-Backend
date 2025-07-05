using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Personal_Phone_Numbers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public int Id { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 2)]
        [Unicode]
        public string CountryNumber { get; set; }
        [MaxLength(15)]
        [Required]
        [Unicode]
        public string Number {  get; set; }
        [Required]
        [MaxLength(25)]
        [Unicode]
        public string NumberType { get; set; }
        public Guid PersonalInformation_FK { get; set; }
        public Personal_Information PersonalInformationNA { get; set; }
    }
}
