namespace HR.Application.DTOs.EmployeeLeaveBalances;
public record CreateEmployeeLeaveBalanceDto(long EmployeeId, long LeaveTypeId, int Year, decimal EntitledDays, decimal CarriedForwardDays, decimal UsedDays, decimal AdjustedDays);
