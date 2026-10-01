using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Domain.Enums;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly ApplicationDbContext _context;

    public LeaveRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken) => _context.LeaveRequests.AsNoTracking().OrderByDescending(leaveRequest => leaveRequest.RequestedAt).ToListAsync(cancellationToken);
    public Task<List<LeaveRequest>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => _context.LeaveRequests.AsNoTracking().Where(leaveRequest => leaveRequest.EmployeeId == employeeId).OrderByDescending(leaveRequest => leaveRequest.RequestedAt).ToListAsync(cancellationToken);
    public Task<LeaveRequest?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveRequests.AsNoTracking().FirstOrDefaultAsync(leaveRequest => leaveRequest.Id == id, cancellationToken);
    public Task<LeaveRequest?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveRequests.FirstOrDefaultAsync(leaveRequest => leaveRequest.Id == id, cancellationToken);
    public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken) => _context.LeaveRequests.AnyAsync(leaveRequest => leaveRequest.Id == id, cancellationToken);
    public Task<bool> HasOverlappingAsync(
        long employeeId,
        DateTime startDate,
        DateTime endDate,
        long? excludedRequestId,
        CancellationToken cancellationToken) =>
        _context.LeaveRequests.AnyAsync(
            leaveRequest => leaveRequest.EmployeeId == employeeId
                && (!excludedRequestId.HasValue || leaveRequest.Id != excludedRequestId.Value)
                && (leaveRequest.Status == LeaveRequestStatus.Pending
                    || leaveRequest.Status == LeaveRequestStatus.Approved)
                && leaveRequest.StartDate <= endDate
                && leaveRequest.EndDate >= startDate,
            cancellationToken);

    public Task<decimal> GetPendingRequestedDaysAsync(
        long employeeId,
        long leaveTypeId,
        int year,
        long? excludedRequestId,
        CancellationToken cancellationToken) =>
        _context.LeaveRequests
            .Where(leaveRequest => leaveRequest.EmployeeId == employeeId
                && leaveRequest.LeaveTypeId == leaveTypeId
                && leaveRequest.StartDate.Year == year
                && leaveRequest.Status == LeaveRequestStatus.Pending
                && (!excludedRequestId.HasValue || leaveRequest.Id != excludedRequestId.Value))
            .SumAsync(leaveRequest => leaveRequest.RequestedDays, cancellationToken);
    public async Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken) => await _context.LeaveRequests.AddAsync(leaveRequest, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
