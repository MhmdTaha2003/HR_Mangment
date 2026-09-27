namespace HR.Application.DTOs.Departments;
public record UpdateDepartmentDto(string NameEn, string NameAr, long BranchId, long? ParentDepartmentId, bool IsActive);
