using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IJobTitleRepository
{
    Task<List<JobTitle>> GetAllAsync(CancellationToken cancellationToken);
    Task<JobTitle?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<JobTitle?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(JobTitle jobTitle, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


