using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeAssignmentRepository
{
    Task<List<EmployeeAssignment>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<EmployeeAssignment>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken);
    Task<EmployeeAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeAssignment?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> HasOtherCurrentAsync(long employeeId, long? exceptId, CancellationToken cancellationToken);
    Task<List<long>> GetCurrentDirectReportIdsAsync(long managerEmployeeId, CancellationToken cancellationToken);
    Task<bool> IsCurrentDirectReportAsync(long managerEmployeeId, long employeeId, CancellationToken cancellationToken);
    Task AddAsync(EmployeeAssignment assignment, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


