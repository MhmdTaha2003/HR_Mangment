using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeContractRepository
{
    Task<List<EmployeeContract>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<EmployeeContract>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken);
    Task<EmployeeContract?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeContract?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> NumberExistsAsync(string number, CancellationToken cancellationToken);
    Task AddAsync(EmployeeContract contract, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


