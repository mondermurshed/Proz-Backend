namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class GettingStartedStageTwoDTO
    {
        public string AdminEmail { get; set; }
        public string Code { get; set; }


        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public DateOnly Date_Of_Birth { get; set; }
        public string Gender { get; set; }
        public string? Nationality { get; set; }
        public bool Living_On_Primary_Place { get; set; } = true;


    }
}
