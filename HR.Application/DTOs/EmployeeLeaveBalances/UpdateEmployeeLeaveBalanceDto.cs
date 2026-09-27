namespace HR.Application.DTOs.EmployeeLeaveBalances;
public record UpdateEmployeeLeaveBalanceDto(decimal EntitledDays, decimal CarriedForwardDays, decimal UsedDays, decimal AdjustedDays);
