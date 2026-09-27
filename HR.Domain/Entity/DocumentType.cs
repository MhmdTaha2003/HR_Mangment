using HR.Domain.Common;

namespace HR.Domain.Entity;

public class DocumentType : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public bool RequiresExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<EmployeeDocument> EmployeeDocuments { get; set; } = new List<EmployeeDocument>();
}
