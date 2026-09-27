namespace HR.Application.DTOs.EmployeeDocuments;
public record UpdateEmployeeDocumentDto(long DocumentTypeId, string? DocumentNumber, DateTime? IssueDate, DateTime? ExpiryDate, string FilePath, string? Notes);
