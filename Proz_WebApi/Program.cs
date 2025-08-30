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
using Proz_WebApi.Helpers_Types;
using System.ComponentModel;
using Proz_WebApi.Helpers_Services;
using EasyCaching.Core.Configurations;
using DnsClient;
using Amazon.SimpleEmailV2;
using Amazon.Extensions.NETCore.Setup;
using Proz_WebApi.Services.DesktopServices;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;


using StackExchange.Redis;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.SignalR;
using Proz_WebApi.Helpers_Services.SignleR_Logic;

var builder = WebApplication.CreateBuilder(args);

Maps.RegisterMappings();//noticed that Maps is an actual staic class that we have created ourselfs in the helpers folder, the RegisterMappings() is our static method that we have defined inside the Maps class. The program.cs is considered as or Main method so the code starts from here, and we want to call this method in every time we run the project (and of course before any request process) so mapster will know the golbal settings for its mapping process that it will done.


builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActivityFilter>();
})
.AddNewtonsoftJson(); //please add this ".AddNewtonsoftJson()" in here when you install the packages

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
builder.Services.AddScoped<HRLogicService>();

builder.Services.AddScoped<EmployeeLogicService>();
//builder.Services.AddSingleton<ILookupNormalizer, EmailNormalizer>();
builder.Services.AddScoped<EmailNormalizer>();
builder.Services.AddScoped<DomainVerifier>();

builder.Services.AddScoped<VerificationCodeService>();
builder.Services.AddScoped<LookupClient>();

builder.Services.AddSingleton<AesEncryptionService>();
builder.Services.AddSingleton(PollyPolicyRegistry.CreateDefaultRetryPolicy());
builder.Services.AddSingleton<SesEmailSender>();
builder.Services.AddHttpContextAccessor(); //this is to register (add to DI) this interface. This is all you need — it registers IHttpContextAccessor as a singleton, which is how it’s supposed to work. This service is used to get the IP address of the user from the HTTP/HTTPs request.
//Scoped Transient Singleton
//----------------------------------------------------------------------------------------

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions()); // Load AWS config from appsettings.json. This just takes all the settings that you have defined inside the app.setting.json file in a section that says "AWS" AND LOAD THESE SETTINGS.
builder.Services.AddAWSService<IAmazonSimpleEmailServiceV2>(); //// Add Amazon SES client


//for AWS setup
//----------------------------------------------------------------------------------------
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // If the token is provided as a query string parameter named "access_token" (common for SignalR JS clients)
            var accessToken = context.Request.Query["access_token"].FirstOrDefault();
            var path = context.HttpContext.Request.Path;
            // If this is the path of your hub, use the token from query string
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/role"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, SubOrNameIdUserIdProvider>();
//for SignalR logic
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------





//for Jwt Configuration
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<Program>(); // Scans for validators. •	This scans the assembly containing the Program class (your main entry point) for any classes that implement IValidator<T>. Validators are typically defined as separate classes that inherit from AbstractValidator<T> and specify validation rules for a specific model type.



        fv.AutomaticValidationEnabled = true; // Auto-validate models. •	This enables automatic validation of models during the request pipeline. •	When a controller action receives a model(e.g., via[FromBody]), FluentValidation will automatically validate it before the action executes. •	If validation fails, the framework will return a 400 Bad Request response with the validation errors.


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
            Message = "Validation errors occurred.",
            Errors = errors
        });
    };
});
//for FluentValidation
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------

builder.Services.AddDbContext<ApplicationDbContext_Desktop>(option => //for desktop
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DesktopDBSqlConnection"));
});

builder.Services.AddDbContext<ApplicationDbContext_WebApp>(option => //for web app
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("WebAppDBSqlConnection"));
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
builder.Services.AddIdentity<ExtendedIdentityUsersDesktop, ExtendedIdentityRolesDesktop>(options =>
{
    // Username settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    options.User.RequireUniqueEmail = false; //this let the framework accepts only unique emails that wasn't defined before in the system
 
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; //if it's true then it will Forces passwords to include at least one symbol that is not a letter or number (e.g., !, @, #, $, %, etc.).
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 2; //this will require the password to have atleast two unique characters, like user can't enter "AAAAAAA" as a password but can enter "AAAAAF1"
 
    // Lockout settings (THESE settings are preventing brute-force attacks by the users
    options.Lockout.MaxFailedAccessAttempts = 10; //if the user enters their password wrong 5 times in a row then their account will be locked
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); //this control the time that their accounts will be locked for 
    options.Lockout.AllowedForNewUsers = true; //this mean apply these rules (the MaxFailedAccessAttempts and DefaultLockoutTimeSpan) even to the new registerd users.


}).AddEntityFrameworkStores<ApplicationDbContext_Desktop>()
.AddDefaultTokenProviders(); //this is required for identity's token systen to work like the token that identity package generate to reset the user's password. If this wasn't here identity will not be able to do these features.

builder.Services.Configure<DataProtectionTokenProviderOptions>(option =>
{
    option.TokenLifespan = TimeSpan.FromMinutes(30); //we are making any tokens that identity generate to live 30 minutes (the default was 1 hour)
});
//For identity UseAuthentication
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
     builder.Services.AddAuthorization(options => //you can get this Authorization system from Microsoft.AspNetCore.Authorization package, which is automatically included when you: Create an ASP.NET Core project or when you Install the Microsoft.AspNetCore.Identity package. No extra NuGet packages needed!
     {
         // Policy using Identity roles
         options.AddPolicy("AdminOrHR", policy =>
         {
             policy.RequireRole(AppRoles_Desktop.Admin, AppRoles_Desktop.HRManager);
         });

         options.AddPolicy("Admin", policy =>
     {
         policy.RequireRole(AppRoles_Desktop.Admin);
        
         
     });
       
    
    options.AddPolicy("HRManager", policy =>
        policy.RequireRole(AppRoles_Desktop.HRManager));

    options.AddPolicy("DepartmentManager", policy =>
    {
        policy.RequireRole(AppRoles_Desktop.DepartmentManager);
        //policy.RequireClaim("DepartmentApproved", "true");
    });
    options.AddPolicy("Employee", policy =>
    {
        policy.RequireRole(AppRoles_Desktop.Employee);
        
    });
    options.AddPolicy("User", policy =>
    {
        policy.RequireRole(AppRoles_Desktop.User);

    });

         options.AddPolicy("EDH", policy =>
         {
             policy.RequireRole(AppRoles_Desktop.Employee, AppRoles_Desktop.DepartmentManager, AppRoles_Desktop.HRManager);

         });

         options.AddPolicy("AllUsers", policy =>
     {
         policy.RequireRole(AppRoles_Desktop.User, AppRoles_Desktop.Employee, AppRoles_Desktop.DepartmentManager, AppRoles_Desktop.HRManager, AppRoles_Desktop.Admin);

     });
         options.AddPolicy("AllExceptUsers", policy =>
         {
             policy.RequireRole(AppRoles_Desktop.Employee, AppRoles_Desktop.DepartmentManager, AppRoles_Desktop.HRManager, AppRoles_Desktop.Admin);

         });
         options.AddPolicy("TrustedUser", policy =>
    {
        policy.RequireRole("User", "Moderator");
        policy.RequireClaim("AccountAge", "6Months");
    });
});
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
builder.Services.AddEasyCaching(options =>
{
    options.UseRedis(redisConfig =>
    {
        // Point to your Redis endpoint(s)
        redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
        // redisConfig.DBConfig.Database = 0;  // optional
    }, "redis1");          // name this provider "redis1"
    options.WithSystemTextJson("redis1");


});

//For easy caching configuration.
//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowResetPage", policy =>
//    {
//        policy.WithOrigins("https://reset.prozsupport.xyz") //this will make anyone in public who has the URL in his browser https://reset.prozsupport.xyz to request any endpoint in this application
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//    options.AddPolicy("AllowAPICalls", policy =>
//    {
//        policy.WithOrigins("https://api.prozsupport.xyz") //this will make anyone in public who has the URL in his browser https://reset.prozsupport.xyz to request any endpoint in this application
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});







//CORS Policy
//----------------------------------------------------------------------------------------















//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ExtendedIdentityRolesDesktop>>();
//    //default hex colors, user = #21b559    employee = #2181b5   department manager = #2621b5    hr manager =  #9c21b5   admin = #b52121
//    //var roleName = AppRoles_Desktop.User;
//    //var roleColorCode = "#b52121";
//    var rolenames = new List<string>() { AppRoles_Desktop.User, AppRoles_Desktop.Employee, AppRoles_Desktop.DepartmentManager, AppRoles_Desktop.HRManager, AppRoles_Desktop.Admin };
//    var roleColorValues = new List<string>() { "#21b559", "#2181b5", "#2621b5", "#9c21b5", "#b52121" };
//    int count = 0;
//    foreach (var rolename in rolenames)
//    {
//        var role = new ExtendedIdentityRolesDesktop
//        {
//            Name = rolename,
//            RoleColorCode = roleColorValues[count],
//        };

//        var result = await roleManager.CreateAsync(role);

//        if (!result.Succeeded)
//        {
//            foreach (var error in result.Errors)
//            {
//                Console.WriteLine($"Role creation error: {error.Description}");
//            }
//        }
//        count++;
//    }


//}
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});
app.MapControllers();


app.MapHub<MainHub>("/hubs/Main")/*.RequireAuthorization()*/;

app.Run();





