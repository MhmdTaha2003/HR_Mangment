using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync(CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Department?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(Department department, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


