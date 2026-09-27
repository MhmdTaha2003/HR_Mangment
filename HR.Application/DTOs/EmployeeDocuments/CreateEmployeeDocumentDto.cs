namespace HR.Application.DTOs.EmployeeDocuments;
public record CreateEmployeeDocumentDto(long EmployeeId, long DocumentTypeId, string? DocumentNumber, DateTime? IssueDate, DateTime? ExpiryDate, string FilePath, string? Notes);
