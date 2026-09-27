using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<DocumentType>> GetAllAsync(CancellationToken cancellationToken) => _context.DocumentTypes.AsNoTracking().Where(documentType => documentType.IsActive).OrderBy(documentType => documentType.Code).ToListAsync(cancellationToken);
    public Task<DocumentType?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(documentType => documentType.Id == id && documentType.IsActive, cancellationToken);
    public Task<DocumentType?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.DocumentTypes.FirstOrDefaultAsync(documentType => documentType.Id == id, cancellationToken);
    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken) => _context.DocumentTypes.AnyAsync(documentType => documentType.Code == code, cancellationToken);
    public async Task AddAsync(DocumentType documentType, CancellationToken cancellationToken) => await _context.DocumentTypes.AddAsync(documentType, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

