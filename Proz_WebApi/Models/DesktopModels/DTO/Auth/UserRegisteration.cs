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

        public string FullName { get; set; }

        public int Age { get; set; }
        public DateOnly Date_Of_Birth { get; set; }
        public string Gender { get; set; }
        public string? Nationality { get; set; }
        public bool Living_On_Primary_Place { get; set; } = true;
    }
}
