using HR.Domain.Common;

namespace HR.Domain.Entity;

public class Department : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public long BranchId { get; set; }
    public long? ParentDepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public Branch Branch { get; set; } = null!;
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
    public ICollection<EmployeeAssignment> EmployeeAssignments { get; set; } = new List<EmployeeAssignment>();
}
