using HR.Domain.Common;
using HR.Domain.Enums;

namespace HR.Domain.Entity;

public class EmployeeContract : BaseAuditableEntity
{
    public long EmployeeId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public ContractType ContractType { get; set; }
    public ContractStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateOnly? ProbationEndDate { get; set; }
    public decimal? Salary { get; set; }
    public string? Notes { get; set; }
    public Employee Employee { get; set; } = null!;
}
