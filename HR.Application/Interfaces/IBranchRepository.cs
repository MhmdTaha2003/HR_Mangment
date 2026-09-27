using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IBranchRepository
{
    Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Branch?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Branch?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Branch branch, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
