namespace HR.Application.DTOs.JobTitles;
public record UpdateJobTitleDto(string NameEn, string NameAr, string? Description, bool IsActive);
