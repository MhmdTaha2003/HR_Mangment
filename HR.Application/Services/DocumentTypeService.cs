using HR.Application.DTOs.Departments;
using HR.Application.DTOs.JobTitles;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;

namespace HR.Application.Services;
public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public DocumentTypeService(
        IDocumentTypeRepository documentTypeRepository)
    {
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<List<DocumentTypeDto>> GetAllAsync(CancellationToken cancellationToken) => (await _documentTypeRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<DocumentTypeDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var documentType = await _documentTypeRepository.GetByIdAsync(id, cancellationToken);
        return documentType is null ? null : Map(documentType);
    }

    public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto dto, CancellationToken cancellationToken)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await _documentTypeRepository.CodeExistsAsync(normalizedCode, cancellationToken))
            throw new InvalidOperationException("Document type code already exists.");
        var documentType = new DocumentType
        {
            Code = normalizedCode,
            NameEn = dto.NameEn.Trim(),
            NameAr = dto.NameAr.Trim(),
            RequiresExpiryDate = dto.RequiresExpiryDate,
            IsActive = true
        };
        await _documentTypeRepository.AddAsync(documentType, cancellationToken);
        await _documentTypeRepository.SaveChangesAsync(cancellationToken);
        return Map(documentType);
    }

    public async Task<bool> UpdateAsync(long id, UpdateDocumentTypeDto dto, CancellationToken cancellationToken)
    {
        var documentType = await _documentTypeRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (documentType is null)
            return false;
        documentType.NameEn = dto.NameEn.Trim();
        documentType.NameAr = dto.NameAr.Trim();
        documentType.RequiresExpiryDate = dto.RequiresExpiryDate;
        documentType.IsActive = dto.IsActive;
        documentType.UpdatedAt = DateTime.UtcNow;
        await _documentTypeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var documentType = await _documentTypeRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (documentType is null)
            return false;
        documentType.IsActive = false;
        documentType.UpdatedAt = DateTime.UtcNow;
        await _documentTypeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static DocumentTypeDto Map(DocumentType documentType) => new(documentType.Id, documentType.Code, documentType.NameEn, documentType.NameAr, documentType.RequiresExpiryDate, documentType.IsActive);
}

