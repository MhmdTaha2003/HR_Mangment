using HR.Application.DTOs.Employees;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class EmployeeDocumentService : IEmployeeDocumentService
{
    private readonly IEmployeeDocumentRepository _employeeDocumentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public EmployeeDocumentService(
        IEmployeeDocumentRepository employeeDocumentRepository,
        IEmployeeRepository employeeRepository,
        IDocumentTypeRepository documentTypeRepository)
    {
        _employeeDocumentRepository = employeeDocumentRepository;
        _employeeRepository = employeeRepository;
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<List<EmployeeDocumentDto>> GetAllAsync(CancellationToken cancellationToken) => (await _employeeDocumentRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<List<EmployeeDocumentDto>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => (await _employeeDocumentRepository.GetByEmployeeIdAsync(employeeId, cancellationToken)).Select(Map).ToList();
    public async Task<EmployeeDocumentDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var employeeDocument = await _employeeDocumentRepository.GetByIdAsync(id, cancellationToken);
        return employeeDocument is null ? null : Map(employeeDocument);
    }

    async Task Check(long emp, long type, DateTime? expiry, CancellationToken cancellationToken)
    {
        if (!await _employeeRepository.ExistsAsync(emp, cancellationToken))
            throw new InvalidOperationException("Employee not found.");
        var documentType = await _documentTypeRepository.GetByIdAsync(type, cancellationToken) ?? throw new InvalidOperationException("Active document type not found.");
        if (documentType.RequiresExpiryDate && !expiry.HasValue)
            throw new InvalidOperationException("Expiry date is required for this document type.");
    }

    public async Task<EmployeeDocumentDto> CreateAsync(CreateEmployeeDocumentDto dto, CancellationToken cancellationToken)
    {
        await Check(dto.EmployeeId, dto.DocumentTypeId, dto.ExpiryDate, cancellationToken);
        var employeeDocument = new EmployeeDocument
        {
            EmployeeId = dto.EmployeeId,
            DocumentTypeId = dto.DocumentTypeId,
            DocumentNumber = dto.DocumentNumber?.Trim(),
            IssueDate = dto.IssueDate,
            ExpiryDate = dto.ExpiryDate,
            FilePath = dto.FilePath.Trim(),
            Notes = dto.Notes?.Trim()
        };
        await _employeeDocumentRepository.AddAsync(employeeDocument, cancellationToken);
        await _employeeDocumentRepository.SaveChangesAsync(cancellationToken);
        return Map(employeeDocument);
    }

    public async Task<bool> UpdateAsync(long id, UpdateEmployeeDocumentDto dto, CancellationToken cancellationToken)
    {
        var employeeDocument = await _employeeDocumentRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (employeeDocument is null)
            return false;
        await Check(employeeDocument.EmployeeId, dto.DocumentTypeId, dto.ExpiryDate, cancellationToken);
        employeeDocument.DocumentTypeId = dto.DocumentTypeId;
        employeeDocument.DocumentNumber = dto.DocumentNumber?.Trim();
        employeeDocument.IssueDate = dto.IssueDate;
        employeeDocument.ExpiryDate = dto.ExpiryDate;
        employeeDocument.FilePath = dto.FilePath.Trim();
        employeeDocument.Notes = dto.Notes?.Trim();
        employeeDocument.UpdatedAt = DateTime.UtcNow;
        await _employeeDocumentRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static EmployeeDocumentDto Map(EmployeeDocument employeeDocument) => new(employeeDocument.Id, employeeDocument.EmployeeId, employeeDocument.DocumentTypeId, employeeDocument.DocumentNumber, employeeDocument.IssueDate, employeeDocument.ExpiryDate, employeeDocument.FilePath, employeeDocument.Notes);
}

