namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class GettingStartedStageOneTemp
    {
        public string AdminUsername { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public string PaymentFrequency { get; set; } = "Every Month";
        public string FullName { get; set; }
        public int Age { get; set; }
        public DateOnly Date_Of_Birth { get; set; }
        public string Gender { get; set; }
        public string? Nationality { get; set; }
        public bool Living_On_Primary_Place { get; set; } = true;
    }
}
