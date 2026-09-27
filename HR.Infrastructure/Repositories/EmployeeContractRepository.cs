using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeContractRepository : IEmployeeContractRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<EmployeeContract>> GetAllAsync(CancellationToken cancellationToken) => _context.EmployeeContracts.AsNoTracking().OrderByDescending(contract => contract.StartDate).ToListAsync(cancellationToken);
    public Task<EmployeeContract?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeContracts.AsNoTracking().FirstOrDefaultAsync(contract => contract.Id == id, cancellationToken);
    public Task<EmployeeContract?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.EmployeeContracts.FirstOrDefaultAsync(contract => contract.Id == id, cancellationToken);
    public Task<bool> NumberExistsAsync(string number, CancellationToken cancellationToken) => _context.EmployeeContracts.AnyAsync(contract => contract.ContractNumber == number, cancellationToken);
    public async Task AddAsync(EmployeeContract contract, CancellationToken cancellationToken) => await _context.EmployeeContracts.AddAsync(contract, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

