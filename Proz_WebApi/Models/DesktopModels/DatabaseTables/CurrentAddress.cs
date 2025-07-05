using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class CurrentAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string CountryName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string CityName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string StreetAddress { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [Unicode]
        public string? DepartmentNumber { get; set; }
        public double? Latitude_Coordinate { get; set; }
        public double? Longitude_Coordinate { get; set; }
        [StringLength(500, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string Describe_The_Location {  get; set; }
        public Guid PersonalInformation_FK { get; set; }
        public Personal_Information PersonalInformationNA { get; set; }
    }
}
