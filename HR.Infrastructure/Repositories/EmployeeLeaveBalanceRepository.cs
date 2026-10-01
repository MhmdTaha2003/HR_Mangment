using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeLeaveBalanceRepository : IEmployeeLeaveBalanceRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeLeaveBalanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<EmployeeLeaveBalance>> GetAllAsync(CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.AsNoTracking().OrderByDescending(leaveBalance => leaveBalance.Year).ToListAsync(cancellationToken);
    public Task<List<EmployeeLeaveBalance>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.AsNoTracking().Where(leaveBalance => leaveBalance.EmployeeId == employeeId).OrderByDescending(leaveBalance => leaveBalance.Year).ToListAsync(cancellationToken);
    public Task<EmployeeLeaveBalance?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.AsNoTracking().FirstOrDefaultAsync(leaveBalance => leaveBalance.Id == id, cancellationToken);
    public Task<EmployeeLeaveBalance?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.FirstOrDefaultAsync(leaveBalance => leaveBalance.Id == id, cancellationToken);
    public Task<EmployeeLeaveBalance?> GetByKeyAsync(long employeeId, long leaveTypeId, int year, CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.AsNoTracking().FirstOrDefaultAsync(leaveBalance => leaveBalance.EmployeeId == employeeId && leaveBalance.LeaveTypeId == leaveTypeId && leaveBalance.Year == year, cancellationToken);
    public Task<EmployeeLeaveBalance?> GetTrackedByKeyAsync(long employeeId, long leaveTypeId, int year, CancellationToken cancellationToken) => _context.EmployeeLeaveBalances.FirstOrDefaultAsync(leaveBalance => leaveBalance.EmployeeId == employeeId && leaveBalance.LeaveTypeId == leaveTypeId && leaveBalance.Year == year, cancellationToken);
    public async Task AddAsync(EmployeeLeaveBalance leaveBalance, CancellationToken cancellationToken) => await _context.EmployeeLeaveBalances.AddAsync(leaveBalance, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
