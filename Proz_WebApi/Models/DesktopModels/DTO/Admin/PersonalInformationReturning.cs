namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class PersonalInformationReturning
    {
        public string FullName { get; set; }
        public int? Age { get; set; }
        public DateOnly? Date_Of_Birth { get; set; }
        public string Gender { get; set; }
        public string? Nationality { get; set; }
        public string Living_On_Primary_Place { get; set; } = "Yes";
        public string RoleName { get; set; } 
        public string? RoleColor { get; set; }
        public string AccountStatus { get; set; }
        public DateOnly AccountCreatedDate { get; set; }
        public DateOnly HireDate { get; set; }
        public int AccountAgeInDays { get; set; }
    }
}
