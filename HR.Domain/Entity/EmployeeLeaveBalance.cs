using HR.Domain.Common;

namespace HR.Domain.Entity;

public class EmployeeLeaveBalance : BaseAuditableEntity
{
    public long EmployeeId { get; set; }
    public long LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal EntitledDays { get; set; }
    public decimal CarriedForwardDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal AdjustedDays { get; set; }
    public decimal RemainingBalance => EntitledDays + CarriedForwardDays + AdjustedDays - UsedDays;
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}
