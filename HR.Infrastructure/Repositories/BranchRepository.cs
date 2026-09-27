using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class BranchRepository : IBranchRepository
{
    private readonly ApplicationDbContext _context;
    public BranchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Branches.AsNoTracking().Where(branch => branch.IsActive).OrderBy(branch => branch.Code).ToListAsync(cancellationToken);
    }

    public async Task<Branch?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Branches.AsNoTracking().FirstOrDefaultAsync(branch => branch.Id == id && branch.IsActive, cancellationToken);
    }

    public async Task<Branch?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Branches.FirstOrDefaultAsync(branch => branch.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Branches.AnyAsync(branch => branch.Code == code, cancellationToken);
    }

    public async Task AddAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await _context.Branches.AddAsync(branch, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
