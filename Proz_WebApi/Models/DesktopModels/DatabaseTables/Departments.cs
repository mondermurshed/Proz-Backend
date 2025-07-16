using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Proz_WebApi.Models.DesktopModels.DatabaseTables
{
    public class Departments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Unicode]
        [Required]
        public string DepartmentName { get; set; } //IT
        public double DepartmentDefaultSalary {  get; set; }
        public Guid Manager_FK { get; set; }
        public Employees ManagerNA { get; set; }
        public Guid? ParentDepartment_FK { get; set; } //If this is null then it's the parent department (it's not a child to any department above it)
        public Departments ParentDepartmentNA { get; set; } //to load the parent of a department (any department inside the department tree) → you need ParentDepartment. The ParentDepartmentId and the ParentDepartment are used together and to go up the hierarchy (one jump) and get the parent of the current department.
        public ICollection<Departments> SubDepartmentsNA { get; set; } = new List<Departments>();
        public ICollection<DepartmentContactMethods> DepartmentContactMethodsNA { get; set; } = new List<DepartmentContactMethods>();

    }
}
