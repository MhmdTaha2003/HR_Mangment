using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class LeaveTypeRepository : ILeaveTypeRepository
{
    private readonly ApplicationDbContext _context;

    public LeaveTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<LeaveType>> GetAllAsync(CancellationToken cancellationToken) => _context.LeaveTypes.AsNoTracking().Where(leaveType => leaveType.IsActive).OrderBy(leaveType => leaveType.Code).ToListAsync(cancellationToken);
    public Task<LeaveType?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(leaveType => leaveType.Id == id && leaveType.IsActive, cancellationToken);
    public Task<LeaveType?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveTypes.FirstOrDefaultAsync(leaveType => leaveType.Id == id, cancellationToken);
    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken) => _context.LeaveTypes.AnyAsync(leaveType => leaveType.Code == code, cancellationToken);
    public async Task AddAsync(LeaveType leaveType, CancellationToken cancellationToken) => await _context.LeaveTypes.AddAsync(leaveType, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

