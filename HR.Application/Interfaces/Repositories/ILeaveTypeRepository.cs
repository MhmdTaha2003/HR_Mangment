using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface ILeaveTypeRepository
{
    Task<List<LeaveType>> GetAllAsync(CancellationToken cancellationToken);
    Task<LeaveType?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<LeaveType?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(LeaveType leaveType, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


