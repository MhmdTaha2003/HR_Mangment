namespace HR.Application.DTOs.EmployeeDocuments;
public record EmployeeDocumentDto(long Id, long EmployeeId, long DocumentTypeId, string? DocumentNumber, DateTime? IssueDate, DateTime? ExpiryDate, string FilePath, string? Notes);
