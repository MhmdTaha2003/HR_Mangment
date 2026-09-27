using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class LeaveRequestApprovalRepository : ILeaveRequestApprovalRepository
{
    private readonly ApplicationDbContext _context;

    public LeaveRequestApprovalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<LeaveRequestApproval>> GetAllAsync(CancellationToken cancellationToken) => _context.LeaveRequestApprovals.AsNoTracking().OrderBy(approval => approval.LeaveRequestId).ThenBy(approval => approval.ApprovalLevel).ToListAsync(cancellationToken);
    public Task<LeaveRequestApproval?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveRequestApprovals.AsNoTracking().FirstOrDefaultAsync(approval => approval.Id == id, cancellationToken);
    public Task<LeaveRequestApproval?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.LeaveRequestApprovals.FirstOrDefaultAsync(approval => approval.Id == id, cancellationToken);
    public Task<bool> LevelExistsAsync(long r, int l, CancellationToken cancellationToken) => _context.LeaveRequestApprovals.AnyAsync(approval => approval.LeaveRequestId == r && approval.ApprovalLevel == l, cancellationToken);
    public async Task AddAsync(LeaveRequestApproval approval, CancellationToken cancellationToken) => await _context.LeaveRequestApprovals.AddAsync(approval, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

