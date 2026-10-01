using FluentValidation;
using HR.Application.Common.Security;
using HR.API.Authorization;
using HR.Application.DTOs.LeaveRequestApprovals;
using HR.Application.Interfaces;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize(Policy = AppPolicies.LeaveApproval)]
public class LeaveRequestApprovalsController : ControllerBase
{
    private readonly ILeaveRequestApprovalService _leaveRequestApprovalService;
    private readonly IValidator<CreateLeaveRequestApprovalDto> _createValidator;
    private readonly IValidator<UpdateLeaveRequestApprovalDto> _updateValidator;

    public LeaveRequestApprovalsController(
        ILeaveRequestApprovalService leaveRequestApprovalService,
        IValidator<CreateLeaveRequestApprovalDto> createValidator,
        IValidator<UpdateLeaveRequestApprovalDto> updateValidator)
    {
        _leaveRequestApprovalService = leaveRequestApprovalService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestApprovalDto>>> GetAll([FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, [FromServices] ILeaveRequestService leaveRequests, CancellationToken cancellationToken)
    {
        var records = await _leaveRequestApprovalService.GetAllAsync(cancellationToken);
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(records);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        var requests = await leaveRequests.GetAllAsync(cancellationToken);
        var allowedRequests = requests.Where(request => ids.Contains(request.EmployeeId)).Select(request => request.Id).ToHashSet();
        return Ok(records.Where(record => allowedRequests.Contains(record.LeaveRequestId)).ToList());
    }
    [HttpGet("{id:long}")]
    public async Task<ActionResult<LeaveRequestApprovalDto>> GetById(long id, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, [FromServices] ILeaveRequestService leaveRequests, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestApprovalService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(result);
        var request = await leaveRequests.GetByIdAsync(result.LeaveRequestId, cancellationToken);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return request is not null && ids.Contains(request.EmployeeId) ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestApprovalDto>> Create(CreateLeaveRequestApprovalDto dto, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, [FromServices] ILeaveRequestService leaveRequests, CancellationToken cancellationToken)
    {
        if (!ManagerTeamAccess.HasFullAccess(User))
        {
            var managerId = await currentUser.GetEmployeeIdAsync();
            if (managerId is not > 0) return Forbid();
            var request = await leaveRequests.GetByIdAsync(dto.LeaveRequestId, cancellationToken);
            if (request is null || request.EmployeeId == managerId || !await teams.IsEmployeeInTeamAsync(managerId.Value, request.EmployeeId, cancellationToken)) return NotFound();
            dto = dto with { ApproverEmployeeId = managerId.Value };
        }
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _leaveRequestApprovalService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateLeaveRequestApprovalDto dto, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, [FromServices] ILeaveRequestService leaveRequests, CancellationToken cancellationToken)
    {
        if (!ManagerTeamAccess.HasFullAccess(User))
        {
            var managerId = await currentUser.GetEmployeeIdAsync();
            if (managerId is not > 0) return Forbid();
            var approval = await _leaveRequestApprovalService.GetByIdAsync(id, cancellationToken);
            if (approval is null || approval.ApproverEmployeeId != managerId) return NotFound();
            var request = await leaveRequests.GetByIdAsync(approval.LeaveRequestId, cancellationToken);
            if (request is null || request.EmployeeId == managerId || !await teams.IsEmployeeInTeamAsync(managerId.Value, request.EmployeeId, cancellationToken)) return NotFound();
        }
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            return await _leaveRequestApprovalService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}

