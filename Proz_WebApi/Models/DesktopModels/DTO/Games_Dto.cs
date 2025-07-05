using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi.Models.DesktopModels.Dto
{
    public class Games_Dto
    {
        //[Range(0, int.MaxValue)]
        //public int id { get; set; }

        public string name { get; set; }
        // [Range(0, double.MaxValue)] Range attribute  is all about a range of the number you want the user to enter in our example the user can enter from 0 to the maximum number that double can hold. So >0 and < of 1.7976931348623157E+308 will accept other then that (if the user entered a negative number which is not making sense) will be blocked.
        public decimal price { get; set; }
        //[Range(0, int.MaxValue)]
        public int sold_copies { get; set; }
        //[StringLength(250)]
        public string description { get; set; }

    }
}
