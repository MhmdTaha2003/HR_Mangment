using HR.Domain.Enums;

namespace HR.Application.DTOs.EmployeeAssignments;
public record UpdateEmployeeAssignmentDto(long BranchId, long DepartmentId, long JobTitleId, long? ManagerEmployeeId, DateTime StartDate, DateTime? EndDate, AssignmentType AssignmentType, bool IsCurrent, string? Notes);
