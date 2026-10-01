using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeDocumentRepository : IEmployeeDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<EmployeeDocument>> GetAllAsync(CancellationToken cancellationToken) => _context.EmployeeDocuments.AsNoTracking().OrderByDescending(employeeDocument => employeeDocument.IssueDate).ToListAsync(cancellationToken);
    public Task<List<EmployeeDocument>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => _context.EmployeeDocuments.AsNoTracking().Where(employeeDocument => employeeDocument.EmployeeId == employeeId).OrderByDescending(employeeDocument => employeeDocument.IssueDate).ToListAsync(cancellationToken);
    public Task<EmployeeDocument?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeDocuments.AsNoTracking().FirstOrDefaultAsync(employeeDocument => employeeDocument.Id == id, cancellationToken);
    public Task<EmployeeDocument?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeDocuments.FirstOrDefaultAsync(employeeDocument => employeeDocument.Id == id, cancellationToken);
    public async Task AddAsync(EmployeeDocument employeeDocument, CancellationToken cancellationToken) => await _context.EmployeeDocuments.AddAsync(employeeDocument, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

