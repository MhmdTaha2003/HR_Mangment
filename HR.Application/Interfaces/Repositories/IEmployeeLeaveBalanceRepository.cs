using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface IEmployeeLeaveBalanceRepository
{
    Task<List<EmployeeLeaveBalance>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeLeaveBalance?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeLeaveBalance?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<EmployeeLeaveBalance?> GetByKeyAsync(long employeeId, long leaveTypeId, int year, CancellationToken cancellationToken);
    Task<EmployeeLeaveBalance?> GetTrackedByKeyAsync(long employeeId, long leaveTypeId, int year, CancellationToken cancellationToken);
    Task AddAsync(EmployeeLeaveBalance leaveBalance, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

