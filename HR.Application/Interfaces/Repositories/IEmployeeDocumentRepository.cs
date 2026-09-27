using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeDocumentRepository
{
    Task<List<EmployeeDocument>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeDocument?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeDocument?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task AddAsync(EmployeeDocument employeeDocument, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


