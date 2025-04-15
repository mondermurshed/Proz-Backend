namespace Proz_WebApi.Models.Dto.Auth
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }  // Links to IdentityUser
        public string TokenHash { get; set; }
        public ExtendedIdentityUsers User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
