using HR.Application.DTOs.EmployeeLeaveBalances;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;
using HR.Domain.Enums;

namespace HR.Application.Services;
public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IEmployeeLeaveBalanceRepository _employeeLeaveBalanceRepository;

    public LeaveRequestService(
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employeeRepository,
        ILeaveTypeRepository leaveTypeRepository,
        IEmployeeLeaveBalanceRepository employeeLeaveBalanceRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _employeeRepository = employeeRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _employeeLeaveBalanceRepository = employeeLeaveBalanceRepository;
    }

    public async Task<List<LeaveRequestDto>> GetAllAsync(CancellationToken cancellationToken) => (await _leaveRequestRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<List<LeaveRequestDto>> GetByEmployeeIdAsync(long employeeId, CancellationToken cancellationToken) => (await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId, cancellationToken)).Select(Map).ToList();
    public async Task<LeaveRequestDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id, cancellationToken);
        return leaveRequest is null ? null : Map(leaveRequest);
    }

    async Task Check(
        long employeeId,
        long leaveTypeId,
        DateTime startDate,
        DateTime endDate,
        decimal requestedDays,
        string? attachmentPath,
        long? excludedRequestId,
        CancellationToken cancellationToken)
    {
        if (!await _employeeRepository.ExistsAsync(employeeId, cancellationToken))
            throw new InvalidOperationException("Employee not found.");
        var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveTypeId, cancellationToken) ?? throw new InvalidOperationException("Active leave type not found.");
        if (leaveType.RequiresAttachment && string.IsNullOrWhiteSpace(attachmentPath))
            throw new InvalidOperationException("An attachment path is required for this leave type.");
        if (await _leaveRequestRepository.HasOverlappingAsync(
                employeeId,
                startDate,
                endDate,
                excludedRequestId,
                cancellationToken))
            throw new InvalidOperationException("The leave request overlaps an existing pending or approved request.");
        var leaveBalance = await _employeeLeaveBalanceRepository.GetByKeyAsync(employeeId, leaveTypeId, startDate.Year, cancellationToken) ?? throw new InvalidOperationException("Leave balance not found for the request year.");
        var pendingRequestedDays = await _leaveRequestRepository.GetPendingRequestedDaysAsync(
            employeeId,
            leaveTypeId,
            startDate.Year,
            excludedRequestId,
            cancellationToken);
        if (leaveBalance.RemainingBalance - pendingRequestedDays < requestedDays)
            throw new InvalidOperationException("Insufficient leave balance.");
    }

    public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        await Check(dto.EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.RequestedDays, dto.AttachmentPath, null, cancellationToken);
        var leaveRequest = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RequestedDays = dto.RequestedDays,
            Reason = dto.Reason?.Trim(),
            AttachmentPath = dto.AttachmentPath?.Trim(),
            Status = LeaveRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
        await _leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);
        await _leaveRequestRepository.SaveChangesAsync(cancellationToken);
        return Map(leaveRequest);
    }

    public async Task<bool> UpdateAsync(long id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (leaveRequest is null)
            return false;
        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be changed.");
        if (dto.Status is not (LeaveRequestStatus.Pending or LeaveRequestStatus.Cancelled))
            throw new InvalidOperationException("A pending request may only remain pending or be cancelled through this operation.");
        await Check(leaveRequest.EmployeeId, leaveRequest.LeaveTypeId, dto.StartDate, dto.EndDate, dto.RequestedDays, dto.AttachmentPath, id, cancellationToken);
        leaveRequest.StartDate = dto.StartDate;
        leaveRequest.EndDate = dto.EndDate;
        leaveRequest.RequestedDays = dto.RequestedDays;
        leaveRequest.Reason = dto.Reason?.Trim();
        leaveRequest.AttachmentPath = dto.AttachmentPath?.Trim();
        leaveRequest.Status = dto.Status;
        leaveRequest.UpdatedAt = DateTime.UtcNow;
        await _leaveRequestRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static LeaveRequestDto Map(LeaveRequest leaveRequest) => new(leaveRequest.Id, leaveRequest.EmployeeId, leaveRequest.LeaveTypeId, leaveRequest.StartDate, leaveRequest.EndDate, leaveRequest.RequestedDays, leaveRequest.Reason, leaveRequest.AttachmentPath, leaveRequest.Status, leaveRequest.RequestedAt);
}
