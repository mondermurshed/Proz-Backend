using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proz_WebApi.Models.DesktopModels.DatabaseTables;

namespace Proz_WebApi.Configurations
{
    public class Employee_DepartmentsConfigurationcs : IEntityTypeConfiguration<Employee_Departments>
    {
        public void Configure(EntityTypeBuilder<Employee_Departments> builder)
        {
            builder.HasKey(ed => ed.Id);

            builder.Property(ed=>ed.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(ed => ed.Salary)
                .HasPrecision(18, 2);

    

            builder.Property(ed => ed.Company_Bonus)
                .HasPrecision(18, 2);


            builder.HasMany(e => e.FeedbacksNA)
               .WithOne(f => f.EmployeeNA)
               .HasForeignKey(f => f.RequesterEmployee_FK);

            builder.HasMany(e => e.LeaveRequestsNA)
               .WithOne(l => l.EmployeeNA)
               .HasForeignKey(l => l.Requester_Employee_FK);

            builder.Property(ed => ed.StartDate)
                .HasColumnType("date");

            builder.HasOne(ed => ed.EmployeeNA)
                .WithMany(ed=>ed.EmployeeToDepatment)
                .HasForeignKey(ed => ed.Employee_FK)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ed => ed.DepartmentNA)
                .WithMany()
                .HasForeignKey(ed => ed.Department_FK);

            builder.HasOne(ed => ed.SalaryScheduleNA)
                .WithOne()
                .HasForeignKey<Salary_Schedule>(s => s.EmployeeDepartment_FK);

            builder.HasMany(ed => ed.PaymentRecordsNA)
                .WithOne(p => p.EmployeeDepartmentsNA)
                .HasForeignKey(p => p.EmployeeDepartments_FK);

            builder.HasMany(ed => ed.PerformanceRecordersNA)
                .WithOne(p => p.EmployeeDepartmentNA)
                .HasForeignKey(p => p.EmployeeDepartment_FK);

            builder.HasMany(ed => ed.AttendanceRecordersNA)
                .WithOne(a => a.EmployeeDepartmentNA)
                .HasForeignKey(a => a.EmployeeDepartment_FK);

            builder.HasMany(ed => ed.EmployeeSalaryHistoryNA)
                .WithOne(eh => eh.EmployeeDepartmentsNA)
                .HasForeignKey(eh => eh.EmployeeDepartments_FK);
        }
    }
}
