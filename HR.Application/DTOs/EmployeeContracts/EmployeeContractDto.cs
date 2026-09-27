using HR.Domain.Enums;

namespace HR.Application.DTOs.EmployeeContracts;
public record EmployeeContractDto(long Id, long EmployeeId, string ContractNumber, ContractType ContractType, ContractStatus Status, DateTime StartDate, DateTime? EndDate, DateOnly? ProbationEndDate, decimal? Salary, string? Notes);
