namespace HR.Application.DTOs.Branches;
public class CreateBranchDto
{
    public string Code { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public string NameAr { get; set; } = null!;
    public string? AddressEn { get; set; }
    public string? AddressAr { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
