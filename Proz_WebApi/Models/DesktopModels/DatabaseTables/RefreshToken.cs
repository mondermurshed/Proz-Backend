namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class RefreshTokenDesktop //one user may have many refresh tokens rows but one refresh token row will have only one user + one device.
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserFK { get; set; }  // Links to IdentityUser
        public ExtendedIdentityUsersDesktop UserNA { get; set; }
        public string TokenHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string? DeviceName { get; set; }  // "Chrome on Windows", "iPhone 13 Safari", etc
        public string DeviceTokenHash { get; set; }
    }
}
