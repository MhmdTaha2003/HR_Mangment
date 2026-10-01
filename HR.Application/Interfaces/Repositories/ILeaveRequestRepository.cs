using HR.Domain.Entity;

namespace HR.Application.Interfaces.Repositories;
public interface ILeaveRequestRepository
{
    Task<List<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<LeaveRequest>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken);
    Task<LeaveRequest?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<LeaveRequest?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
    Task<bool> HasOverlappingAsync(
        long employeeId,
        DateTime startDate,
        DateTime endDate,
        long? excludedRequestId,
        CancellationToken cancellationToken);
    Task<decimal> GetPendingRequestedDaysAsync(
        long employeeId,
        long leaveTypeId,
        int year,
        long? excludedRequestId,
        CancellationToken cancellationToken);
    Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

