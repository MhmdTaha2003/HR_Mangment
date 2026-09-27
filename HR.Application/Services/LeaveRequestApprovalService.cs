using HR.Application.DTOs.EmployeeLeaveBalances;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Entity;
using HR.Domain.Enums;

namespace HR.Application.Services;
public class LeaveRequestApprovalService : ILeaveRequestApprovalService
{
    private readonly ILeaveRequestApprovalRepository _leaveRequestApprovalRepository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeLeaveBalanceRepository _employeeLeaveBalanceRepository;

    public LeaveRequestApprovalService(
        ILeaveRequestApprovalRepository leaveRequestApprovalRepository,
        ILeaveRequestRepository leaveRequestRepository,
        IEmployeeRepository employeeRepository,
        IEmployeeLeaveBalanceRepository employeeLeaveBalanceRepository)
    {
        _leaveRequestApprovalRepository = leaveRequestApprovalRepository;
        _leaveRequestRepository = leaveRequestRepository;
        _employeeRepository = employeeRepository;
        _employeeLeaveBalanceRepository = employeeLeaveBalanceRepository;
    }

    public async Task<List<LeaveRequestApprovalDto>> GetAllAsync(CancellationToken cancellationToken) => (await _leaveRequestApprovalRepository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<LeaveRequestApprovalDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var approval = await _leaveRequestApprovalRepository.GetByIdAsync(id, cancellationToken);
        return approval is null ? null : Map(approval);
    }

    public async Task<LeaveRequestApprovalDto> CreateAsync(CreateLeaveRequestApprovalDto dto, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(dto.LeaveRequestId, cancellationToken)
            ?? throw new InvalidOperationException("Leave request not found.");
        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests may be approved or rejected.");
        if (!await _employeeRepository.ExistsAsync(dto.ApproverEmployeeId, cancellationToken))
            throw new InvalidOperationException("Approver employee not found.");
        if (await _leaveRequestApprovalRepository.LevelExistsAsync(dto.LeaveRequestId, dto.ApprovalLevel, cancellationToken))
            throw new InvalidOperationException("Approval level already exists for this leave request.");
        if (dto.Action != ApprovalAction.Pending)
            throw new InvalidOperationException("A new approval must begin in the pending state.");
        var approval = new LeaveRequestApproval
        {
            LeaveRequestId = dto.LeaveRequestId,
            ApprovalLevel = dto.ApprovalLevel,
            ApproverEmployeeId = dto.ApproverEmployeeId,
            Action = ApprovalAction.Pending,
            Comments = dto.Comments?.Trim()
        };
        await _leaveRequestApprovalRepository.AddAsync(approval, cancellationToken);
        await _leaveRequestApprovalRepository.SaveChangesAsync(cancellationToken);
        return Map(approval);
    }

    public async Task<bool> UpdateAsync(long id, UpdateLeaveRequestApprovalDto dto, CancellationToken cancellationToken)
    {
        var approval = await _leaveRequestApprovalRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (approval is null)
            return false;
        if (approval.Action != ApprovalAction.Pending)
            throw new InvalidOperationException("This approval has already been actioned.");
        if (dto.Action == ApprovalAction.Pending)
            throw new InvalidOperationException("Approval action must be approved or rejected.");
        var leaveRequest = await _leaveRequestRepository.GetTrackedByIdAsync(approval.LeaveRequestId, cancellationToken)
            ?? throw new InvalidOperationException("Leave request not found.");
        if (leaveRequest.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests may be approved or rejected.");

        EmployeeLeaveBalance? leaveBalance = null;
        if (dto.Action == ApprovalAction.Approved)
        {
            leaveBalance = await _employeeLeaveBalanceRepository.GetTrackedByKeyAsync(
                leaveRequest.EmployeeId,
                leaveRequest.LeaveTypeId,
                leaveRequest.StartDate.Year,
                cancellationToken)
                ?? throw new InvalidOperationException("Leave balance not found for the request year.");
            if (leaveBalance.RemainingBalance < leaveRequest.RequestedDays)
                throw new InvalidOperationException("Insufficient leave balance.");
        }

        var actionDate = DateTime.UtcNow;
        approval.Action = dto.Action;
        approval.Comments = dto.Comments?.Trim();
        approval.ActionDate = actionDate;
        approval.UpdatedAt = actionDate;
        leaveRequest.Status = dto.Action == ApprovalAction.Approved
            ? LeaveRequestStatus.Approved
            : LeaveRequestStatus.Rejected;
        leaveRequest.UpdatedAt = actionDate;
        if (leaveBalance is not null)
        {
            leaveBalance.UsedDays += leaveRequest.RequestedDays;
            leaveBalance.UpdatedAt = actionDate;
        }
        await _leaveRequestApprovalRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    static LeaveRequestApprovalDto Map(LeaveRequestApproval approval) => new(approval.Id, approval.LeaveRequestId, approval.ApprovalLevel, approval.ApproverEmployeeId, approval.Action, approval.ActionDate, approval.Comments);
}
