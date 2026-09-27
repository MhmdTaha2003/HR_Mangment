using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync(CancellationToken cancellationToken);
    Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<DocumentType?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(DocumentType documentType, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


