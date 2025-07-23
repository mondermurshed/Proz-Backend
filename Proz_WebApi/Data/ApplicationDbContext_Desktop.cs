using System;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proz_WebApi.Models;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Data
{
    public class ApplicationDbContext_Desktop : IdentityDbContext< //The DBContext is the thing or the worker that will interact with the database for you, so your application doesn't talk with the database directly (like in the ADO.NET) but you talk with your DBContext and it will talk with any database for you like sql server or SQlite ETC.. we defined here IdentityDbContext rather then the regular DbContext because we are working with identity tables.
        ExtendedIdentityUsersDesktop,       // Custom user type
        ExtendedIdentityRolesDesktop,               
        Guid,
        IdentityUserClaim<Guid>,
        ExtendedIdentityUserRolesDesktop, // Custom user roles type (join table which join from the users to roles)
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>> 
    {
        //IdentityDbContext<> The IdentityDbContext requires exactly 8 parameters to work (use IdentityDbContext alone if you don't have extended tables) like in here we tell our identity package which tables to be considered, if we wanna extend a specific identity's table then remove the default one and add your extended class name (table name). Also add them in this way because then if you change the order it will give you error. The third paramter mean the default type of all identity tables this is way we just putted is "string". Also  IdentityRoleClaim<string> the "<string>" means that identifier (the primary key) will be a string type of this whole table which is the default for identity's tables becuase it uses GUID not INT type for primray keys.
        public ApplicationDbContext_Desktop(DbContextOptions<ApplicationDbContext_Desktop> options) : base(options) //did you remember the configuration that we did in the program.cs for the DBContext ? like in there we first did Dependency Injection (DI) so when we call this type "ApplicationDbContext" we get it directly and the framework will create an instance of it automatically without the need to create one ourself and pass it, but we did all of that for any controller of service that wanna work with our DBContext but as you see we also did somthing called options in there! the options's goal is to customize our DBcontext (or any other type of course that uses options) and we tell it how it must work, we did teach our DBcontext where to find our sql server instance and how to connect to it. And now in this class's constructer we got the type DbContextOption directly because it's part of the DI container (it's registered already), so we were able to get the options our ours easily. if you wonder why we put all of our configuration in the program.cs file, it's because it's the entry of our program.
        {
                
        }
        public DbSet<Games_Model> Games { get; set; }
        public DbSet<RefreshTokenDesktop> RefreshTokensTable { get; set; }
        public DbSet<Attendance_Recorder> AttendanceRecorder { get; set; }
        public DbSet<Audit_Logs> AuditLogsTable { get; set; }
        public DbSet<Breaks_Time> BreaksTimeTable { get; set; }
     
        public DbSet<Departments> DepartmentsTable { get; set; }
        public DbSet<Employee_Departments> EmployeeDepartmentsTable { get; set; }
        public DbSet<Employees> EmployeesTable { get; set; }
        public DbSet<EmployeeSalaryHistory> EmployeeSalaryHistoryTable { get; set; }
        public DbSet<Feedbacks> FeedbacksTable { get; set; }
        public DbSet<Feedbacks_Answers> FeedbacksAnswersTable { get; set; }
     
        public DbSet<LeaveRequests> LeaveRequestsTable { get; set; }
        public DbSet<LoginHistory> LoginHistoryTable { get; set; }
        public DbSet<Notifications> NotificationsTable { get; set; }
        public DbSet<Payment_Records> PaymentRecordsTable { get; set; }
        public DbSet<Performance_Recorder> PerformanceRecorderTable { get; set; }
        public DbSet<Personal_Information> PersonalInformationTable { get; set; }
     
        public DbSet<ShiftInformationTable> ShiftInformationTable { get; set; }
        public DbSet<Salary_Schedule> SalaryScheduleTable { get; set; }
        public DbSet<Feedback_Types> FeedbacksTypesTable { get; set; }
        public DbSet<LeaveRequestsHigherRole> LeaveRequestsHigherRoleTable { get; set; }
        public DbSet<GettingStartedTable> GettingStartedTable { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext_Desktop).Assembly);
        


      

     
        }
    }
}
