using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi.Models.Dto
{
    public class Games_Dto2
    {
        [Range(0, int.MaxValue)]
        public int id { get; set; }
        [Required]
        [StringLength(100)]
        public string name { get; set; }
        [Range(0, double.MaxValue)]
        public double price { get; set; }
        [Range(0, int.MaxValue)]
        public int sold_copies { get; set; }
        [StringLength(250)]
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
