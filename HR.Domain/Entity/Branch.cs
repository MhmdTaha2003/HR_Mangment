using HR.Domain.Common;

namespace HR.Domain.Entity;

public class Branch : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public string? AddressEn { get; set; }
    public string? AddressAr { get; set; }

    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<EmployeeAssignment> EmployeeAssignments { get; set; } = new List<EmployeeAssignment>();
}
