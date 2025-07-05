namespace Proz_WebApi.Models.WebAppModels.DatabaseTables
{
    public class RefreshTokenWebApp
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }  // Links to IdentityUser
        public string TokenHash { get; set; }
        public ExtendedIdentityUsersWebApp UserNA { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
