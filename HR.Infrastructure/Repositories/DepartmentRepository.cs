using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Department>> GetAllAsync(CancellationToken cancellationToken) => _context.Departments.AsNoTracking().Where(department => department.IsActive).OrderBy(department => department.Code).ToListAsync(cancellationToken);
    public Task<Department?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.Departments.AsNoTracking().FirstOrDefaultAsync(department => department.Id == id && department.IsActive, cancellationToken);
    public Task<Department?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.Departments.FirstOrDefaultAsync(department => department.Id == id, cancellationToken);
    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken) => _context.Departments.AnyAsync(department => department.Code == code, cancellationToken);
    public async Task AddAsync(Department department, CancellationToken cancellationToken) => await _context.Departments.AddAsync(department, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

