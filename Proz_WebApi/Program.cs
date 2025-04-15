using System.Threading;
using System.Threading.RateLimiting;
using AspNetCoreRateLimit;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi;
using Proz_WebApi.Controllers;
using Proz_WebApi.Data;
using Proz_WebApi.filters;
using Proz_WebApi.Services;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Zxcvbn;
using Proz_WebApi.Models;
using Proz_WebApi.Helpers_Types;
using System.ComponentModel;
var builder = WebApplication.CreateBuilder(args);

Maps.RegisterMappings();//noticed that Maps is an actual staic class that we have created ourselfs in the helpers folder, the RegisterMappings() is our static method that we have defined inside the Maps class. The program.cs is considered as or Main method so the code starts from here, and we want to call this method in every time we run the project (and of course before any request process) so mapster will know the golbal settings for its mapping process that it will done.

builder.Services.AddControllers().AddNewtonsoftJson();//please add this ".AddNewtonsoftJson()" in here when you install the packages
builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActivityFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//here there is something very fun called Dependency Injection (DI) which is let ASP.NET automatically creates as well as pass object to.
//for example in the constructor of the controller there is some types are defined in there, but did we pass any objects ? NO! but if no then from where does the objects (or instances) are coming from ?
//this is what DI is, look at the three methods down here, these methods are the methods the register these types inside something called "DI container" which is any thing call these types again will ASP.NET will automatically create instances for them without the need for the programmer to create and pass manually.
//Now see down you will see three methods, each method will define how to do the job :
//builder.Services.AddTransient<GamesService>();  //this will create an instance of GamesService every time something needs it (each instance is dependent from each other and each will go to the thing that needs it, like controller or service or anything)
//builder.Services.AddScoped<GamesService>(); //this one is the default one that means it will create an instance for each thing needs it (for each request only)
//builder.Services.AddSingleton<GamesService>(); //this will create only single instance for the whole application, so every thing needs this type will have to deal with the same instacne they everyone is dealing with it already.
builder.Services.AddScoped<GamesService>();
var JWTOptions=builder.Configuration.GetSection("Jwt").Get<JWTOptions>(); //This is just to map our data from anywhere to our JWTOptions class like from JSON files like appsetting or from even system environment variables etc.., in this situation we have mapped the hardcoded data from the appsetting.JSON file to our properties that is in the JWTOptions, so we can use these data by using this class so we don't hardcoded data again and this will help us when we want to change the data we change in only one place and it will be changed in all of them. 
builder.Services.AddSingleton(JWTOptions); //this is to register the actual service so the framework pass all we need when needing this type automatically without the need to do it manually (pass it ourselfs). Noticed that we haven't used the builder.Services.AddSingleton<JWTOptions>(); way because this way will register a new service called JWTOptions but will set the values of the properties to their default (0 for int or null for strings or any complex types) so you should use this way to register the service as well as keep the values from the mapping process.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminLogicService>();
builder.Services.AddScoped<DepartmentManagerLogicService>();
//----------------------------------------------------------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>  // Chain JWT config here
{
    options.SaveToken = true;
    
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = JWTOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = JWTOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(JWTOptions.SigningKey)),
        ValidateLifetime = true, //by making this true you will check all the up coming tokens's expiration, the up to dates tokens will not be allowed entering the system
        ClockSkew = TimeSpan.Zero, //noramlly when a JWT token expiration time ends then the token will still work (because it's valid still) because of this clockskew, by default it's 5m means that even if it's expired at 12AM then it can still be used at maximum period of 12:05AM. We set this to zero so when it's expired it will not be used no more. 
         RequireExpirationTime = true //this will add another layer of security and allow ONLY the valid tokens that their expirationTime wasn't finished yes (this makes it required)
    };
});

//for Jwt Configuration
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<Program>(); // Scans for validators
        fv.AutomaticValidationEnabled = true; // Auto-validate models
        //these two will register all the Validators from the FluentValidation automatically for you.
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Any())
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
              
            );

        return new BadRequestObjectResult(new
        {
            StatusCode = 400,
            add = "asasf",
            Message = "Validation errors occurred.",
            Errors = errors
        });
    };
});
//for FluentValidation
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection"));
});
//this is for sqlserver 
//----------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    //.Filter.ByExcluding(Matching.WithProperty<int>("Count", p => p < 10))
    .WriteTo.Console(/*restrictedToMinimumLevel: LogEventLevel.Debug*/)
    //.WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day, 
    // outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
     builder.Host.UseSerilog(Log.Logger);
//For logging service purposes
//----------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// Add services for rate limiting
builder.Services.AddMemoryCache(); // Required to store rate limit counters
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>(); 
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//for RateLimiting Middleware configuration
//----------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------
builder.Services.AddIdentity<ExtendedIdentityUsers, IdentityRole>(options =>
{
    // Username settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    options.User.RequireUniqueEmail = true; //this let the framework accepts only unique emails that wasn't defined before in the system
 
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; //if it's true then it will Forces passwords to include at least one symbol that is not a letter or number (e.g., !, @, #, $, %, etc.).
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 2; //this will require the password to have atleast two unique characters, like user can't enter "AAAAAAA" as a password but can enter "AAAAAF1"

    // Lockout settings (THESE settings are preventing brute-force attacks by the users
    options.Lockout.MaxFailedAccessAttempts = 5; //if the user enters their password wrong 5 times in a row then their account will be locked
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); //this control the time that their accounts will be locked for 
    options.Lockout.AllowedForNewUsers = true; //this mean apply these rules (the MaxFailedAccessAttempts and DefaultLockoutTimeSpan) even to the new registerd users.


}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//For identity UseAuthentication
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
builder.Services.AddAuthorization(options =>
{
// Policy using Identity roles
options.AddPolicy("AdminOnly", policy =>
{
    policy.RequireRole(AppRoles.Admin);
   
    
});
       
    
   

    options.AddPolicy("HRManagement", policy =>
        policy.RequireRole(AppRoles.Admin, AppRoles.HRManager));

    options.AddPolicy("DepartmentManagement", policy =>
    {
        policy.RequireRole(AppRoles.Admin, AppRoles.HRManager, AppRoles.DepartmentManager);
        //policy.RequireClaim("DepartmentApproved", "true");
    });

    options.AddPolicy("TrustedUser", policy =>
    {
        policy.RequireRole("User", "Moderator");
        policy.RequireClaim("AccountAge", "6Months");
    });
});
//----------------------------------------------------------------------------------------
var app = builder.Build();



//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    List<string> roles = new List<string> { AppRoles.Admin, AppRoles.HRManager, AppRoles.DepartmentManager, AppRoles.Employee, AppRoles.User }; // add more roles as needed

//    foreach (var role in roles)
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }

//}


//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var role = await roleManager.FindByNameAsync("User");
//    if (role != null)
//    {
//        role.Name = "Employee";
//        var result = await roleManager.UpdateAsync(role);
//        if (!result.Succeeded)
//        {
//            // Handle errors here
//            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
//            throw new Exception($"Role update failed: {errors}");
//        }
//    }
//}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();





