using HR.Domain.Common;

namespace HR.Domain.Entity;

public class EmployeeDocument : BaseAuditableEntity
{
    public long EmployeeId { get; set; }
    public long DocumentTypeId { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Employee Employee { get; set; } = null!;
    public DocumentType DocumentType { get; set; } = null!;
}
