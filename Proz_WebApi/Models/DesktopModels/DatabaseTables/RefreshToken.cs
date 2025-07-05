namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class RefreshTokenDesktop //one user may have many refresh tokens but one refresh token will have only one user.
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserFK { get; set; }  // Links to IdentityUser
        public ExtendedIdentityUsersDesktop UserNA { get; set; }
        public string TokenHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
