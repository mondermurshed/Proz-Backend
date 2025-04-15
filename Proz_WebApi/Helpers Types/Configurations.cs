using System.ComponentModel.DataAnnotations;

namespace Proz_WebApi
{
    //public class AppSettings
    //{
    //    [Required]
    //    public DatabaseSettings database { get; set; } //by this you can access the DatabaseSettings class in any controller.
    //    public JWTOptions jwtoptions { get; set; }
    //}

    //public class DatabaseSettings
    //{
    //    [Required]
    //    public string ConnectionString { get; set; }
    //}
    public class JWTOptions
{
        public string Issuer {  get; set; }
        public string Audience {  get; set; }
        public int LifetimeMinutes { get; set; }
        public string SigningKey { get; set; }
} 
}


