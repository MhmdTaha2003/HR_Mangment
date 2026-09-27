using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Employee?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
    Task<bool> NumberExistsAsync(string number, CancellationToken cancellationToken);
    Task AddAsync(Employee employee, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


