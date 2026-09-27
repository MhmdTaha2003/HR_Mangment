namespace HR.Application.DTOs.DocumentTypes;
public record DocumentTypeDto(long Id, string Code, string NameEn, string NameAr, bool RequiresExpiryDate, bool IsActive);
