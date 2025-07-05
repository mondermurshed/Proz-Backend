using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi.Models.DesktopModels.DTO.HRManager
{
    public class RemoveUsersDTO
    {
        //    [Required]
        //    [MinLength(1, ErrorMessage = "At least one user ID required")]
        public List<Guid> UserIDs { get; set; }

        //[Required]

    }
}
