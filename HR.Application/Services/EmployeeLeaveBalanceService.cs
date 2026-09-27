using HR.Application.DTOs.EmployeeLeaveBalances;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;
using HR.Domain.Enums;

namespace HR.Application.Services;
public class EmployeeLeaveBalanceService : IEmployeeLeaveBalanceService
{
    private readonly IEmployeeLeaveBalanceRepository _employeeLeaveBalanceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;

    public EmployeeLeaveBalanceService(
        IEmployeeLeaveBalanceRepository employeeLeaveBalanceRepository,
        IEmployeeRepository employeeRepository,
        ILeaveTypeRepository leaveTypeRepository)
    {
        _employeeLeaveBalanceRepository = employeeLeaveBalanceRepository;
        _employeeRepository = employeeRepository;
        _leaveTypeRepository = leaveTypeRepository;
    }

    public async Task<List<EmployeeLeaveBalanceDto>> GetAllAsync(CancellationToken cancellationToken) => (await _employeeLeaveBalanceRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<EmployeeLeaveBalanceDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var leaveBalance = await _employeeLeaveBalanceRepository.GetByIdAsync(id, cancellationToken);
        return leaveBalance is null ? null : Map(leaveBalance);
    }

    public async Task<EmployeeLeaveBalanceDto> CreateAsync(CreateEmployeeLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        if (!await _employeeRepository.ExistsAsync(dto.EmployeeId, cancellationToken))
            throw new InvalidOperationException("Employee not found.");
        if (await _leaveTypeRepository.GetByIdAsync(dto.LeaveTypeId, cancellationToken) is null)
            throw new InvalidOperationException("Active leave type not found.");
        if (await _employeeLeaveBalanceRepository.GetByKeyAsync(dto.EmployeeId, dto.LeaveTypeId, dto.Year, cancellationToken) is not null)
            throw new InvalidOperationException("Leave balance already exists for this employee, leave type, and year.");
        var leaveBalance = new EmployeeLeaveBalance
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            Year = dto.Year,
            EntitledDays = dto.EntitledDays,
            CarriedForwardDays = dto.CarriedForwardDays,
            UsedDays = dto.UsedDays,
            AdjustedDays = dto.AdjustedDays
        };
        await _employeeLeaveBalanceRepository.AddAsync(leaveBalance, cancellationToken);
        await _employeeLeaveBalanceRepository.SaveChangesAsync(cancellationToken);
        return Map(leaveBalance);
    }

    public async Task<bool> UpdateAsync(long id, UpdateEmployeeLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        var leaveBalance = await _employeeLeaveBalanceRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (leaveBalance is null)
            return false;
        leaveBalance.EntitledDays = dto.EntitledDays;
        leaveBalance.CarriedForwardDays = dto.CarriedForwardDays;
        leaveBalance.UsedDays = dto.UsedDays;
        leaveBalance.AdjustedDays = dto.AdjustedDays;
        leaveBalance.UpdatedAt = DateTime.UtcNow;
        await _employeeLeaveBalanceRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static EmployeeLeaveBalanceDto Map(EmployeeLeaveBalance leaveBalance) => new(leaveBalance.Id, leaveBalance.EmployeeId, leaveBalance.LeaveTypeId, leaveBalance.Year, leaveBalance.EntitledDays, leaveBalance.CarriedForwardDays, leaveBalance.UsedDays, leaveBalance.AdjustedDays, leaveBalance.RemainingBalance);
}

