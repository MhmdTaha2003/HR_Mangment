using HR.Domain.Common;
using HR.Domain.Enums;

namespace HR.Domain.Entity;

public class EmployeeAssignment : BaseAuditableEntity
{
    public long EmployeeId { get; set; }
    public long BranchId { get; set; }
    public long DepartmentId { get; set; }
    public long JobTitleId { get; set; }
    public long? ManagerEmployeeId { get; set; }
    public AssignmentType AssignmentType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Notes { get; set; }
    public Employee Employee { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public JobTitle JobTitle { get; set; } = null!;
    public Employee? ManagerEmployee { get; set; }
}
