using HR.Domain.Enums;

namespace HR.Application.DTOs.EmployeeAssignments;
public record EmployeeAssignmentDto(long Id, long EmployeeId, long BranchId, long DepartmentId, long JobTitleId, long? ManagerEmployeeId, DateTime StartDate, DateTime? EndDate, AssignmentType AssignmentType, bool IsCurrent, string? Notes);
