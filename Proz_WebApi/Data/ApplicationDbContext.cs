using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Models;

namespace Proz_WebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext //The DBContext is the thing or the worker that will interact with the database for you, so your application doesn't talk with the database directly (like in the ADO.NET) but you talk with your DBContext and it will talk with any database for you like sql server or SQlite ETC..
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) //did you remember the configuration that we did in the program.cs for the DBContext ? like in there we first did Dependency Injection (DI) so when we call this type "ApplicationDbContext" we get it directly and the framework will create an instance of it automatically without the need to create one ourself and pass it, but we did all of that for any controller of service that wanna work with our DBContext but as you see we also did somthing called options in there! the options's goal is to customize our DBcontext (or any other type of course that uses options) and we tell it how it must work, we did teach our DBcontext where to find our sql server instance and how to connect to it. And now in this class's constructer we got the type DbContextOption directly because it's part of the DI container (it's registered already), so we were able to get the options our ours easily. if you wonder why we put all of our configuration in the program.cs file, it's because it's the entry of our program.
        {
                
        }
        public DbSet<Games_Model> Games { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
