namespace HR.Application.DTOs.JobTitles;
public record JobTitleDto(long Id, string Code, string NameEn, string NameAr, string? Description, bool IsActive);
