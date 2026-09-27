using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class JobTitleRepository : IJobTitleRepository
{
    private readonly ApplicationDbContext _context;

    public JobTitleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<JobTitle>> GetAllAsync(CancellationToken cancellationToken) => _context.JobTitles.AsNoTracking().Where(jobTitle => jobTitle.IsActive).OrderBy(jobTitle => jobTitle.Code).ToListAsync(cancellationToken);
    public Task<JobTitle?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.JobTitles.AsNoTracking().FirstOrDefaultAsync(jobTitle => jobTitle.Id == id && jobTitle.IsActive, cancellationToken);
    public Task<JobTitle?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.JobTitles.FirstOrDefaultAsync(jobTitle => jobTitle.Id == id, cancellationToken);
    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken) => _context.JobTitles.AnyAsync(jobTitle => jobTitle.Code == code, cancellationToken);
    public async Task AddAsync(JobTitle jobTitle, CancellationToken cancellationToken) => await _context.JobTitles.AddAsync(jobTitle, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

