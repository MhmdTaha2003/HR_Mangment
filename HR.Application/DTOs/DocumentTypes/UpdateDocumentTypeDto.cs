namespace HR.Application.DTOs.DocumentTypes;
public record UpdateDocumentTypeDto(string NameEn, string NameAr, bool RequiresExpiryDate, bool IsActive);
