using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Azure.Identity;

namespace Proz_WebApi.Models.DesktopModels.Dto.Auth
{

    public class UserRegisteration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
