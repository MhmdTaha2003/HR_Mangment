using HR.Domain.Enums;

namespace HR.Application.DTOs.EmployeeContracts;
public record CreateEmployeeContractDto(long EmployeeId, string ContractNumber, ContractType ContractType, ContractStatus Status, DateTime StartDate, DateTime? EndDate, DateOnly? ProbationEndDate, decimal? Salary, string? Notes);
