namespace HR.Application.DTOs.Departments;
public record CreateDepartmentDto(string Code, string NameEn, string NameAr, long BranchId, long? ParentDepartmentId);
