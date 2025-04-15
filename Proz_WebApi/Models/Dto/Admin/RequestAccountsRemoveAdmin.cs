using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi.Models.Dto.Admin
{
    public class RequestAccountsRemoveAdmin
    {
        //    [Required]
        //    [MinLength(1, ErrorMessage = "At least one user ID required")]
        public List<string> UserIDs { get; set; }

        //[Required]

    }
}
