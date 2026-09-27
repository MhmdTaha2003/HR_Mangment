using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeAssignmentRepository : IEmployeeAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<EmployeeAssignment>> GetAllAsync(CancellationToken cancellationToken) => _context.EmployeeAssignments.AsNoTracking().OrderByDescending(assignment => assignment.StartDate).ToListAsync(cancellationToken);
    public Task<EmployeeAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeAssignments.AsNoTracking().FirstOrDefaultAsync(assignment => assignment.Id == id, cancellationToken);
    public Task<EmployeeAssignment?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeAssignments.FirstOrDefaultAsync(assignment => assignment.Id == id, cancellationToken);
    public Task<bool> HasOtherCurrentAsync(long employeeId, long? exceptId, CancellationToken cancellationToken) => _context.EmployeeAssignments.AnyAsync(assignment => assignment.EmployeeId == employeeId && assignment.IsCurrent && (!exceptId.HasValue || assignment.Id != exceptId), cancellationToken);
    public async Task AddAsync(EmployeeAssignment assignment, CancellationToken cancellationToken) => await _context.EmployeeAssignments.AddAsync(assignment, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

