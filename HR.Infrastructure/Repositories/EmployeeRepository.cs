using HR.Application.Interfaces.Repositories;
using HR.Domain.Entity;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken) => _context.Employees.AsNoTracking().OrderBy(employee => employee.EmployeeNumber).ToListAsync(cancellationToken);
    public Task<Employee?> GetByIdAsync(long id, CancellationToken cancellationToken) => _context.Employees.AsNoTracking().FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    public Task<Employee?> GetTrackedByIdAsync(long id, CancellationToken cancellationToken) => _context.Employees.FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken) => _context.Employees.AnyAsync(employee => employee.Id == id, cancellationToken);
    public Task<bool> NumberExistsAsync(string number, CancellationToken cancellationToken) => _context.Employees.AnyAsync(employee => employee.EmployeeNumber == number, cancellationToken);
    public async Task AddAsync(Employee employee, CancellationToken cancellationToken) => await _context.Employees.AddAsync(employee, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

