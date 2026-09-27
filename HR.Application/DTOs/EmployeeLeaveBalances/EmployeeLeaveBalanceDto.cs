namespace HR.Application.DTOs.EmployeeLeaveBalances;
public record EmployeeLeaveBalanceDto(long Id, long EmployeeId, long LeaveTypeId, int Year, decimal EntitledDays, decimal CarriedForwardDays, decimal UsedDays, decimal AdjustedDays, decimal RemainingBalance);
