using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface ILeaveRequestApprovalRepository
{
    Task<List<LeaveRequestApproval>> GetAllAsync(CancellationToken cancellationToken);
    Task<LeaveRequestApproval?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<LeaveRequestApproval?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> LevelExistsAsync(long requestId, int level, CancellationToken cancellationToken);
    Task AddAsync(LeaveRequestApproval approval, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


