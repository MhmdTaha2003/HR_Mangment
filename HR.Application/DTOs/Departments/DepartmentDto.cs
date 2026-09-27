namespace HR.Application.DTOs.Departments;
public record DepartmentDto(long Id, string Code, string NameEn, string NameAr, long BranchId, long? ParentDepartmentId, bool IsActive);
