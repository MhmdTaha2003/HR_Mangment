using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeAssignmentRepository
{
    Task<List<EmployeeAssignment>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeAssignment?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> HasOtherCurrentAsync(long employeeId, long? exceptId, CancellationToken cancellationToken);
    Task AddAsync(EmployeeAssignment assignment, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


