using FluentValidation;
using HR.Application.Common.Security;
using HR.Application.DTOs.LeaveRequestApprovals;
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
    public async Task<ActionResult<List<LeaveRequestApprovalDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _leaveRequestApprovalService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<LeaveRequestApprovalDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestApprovalService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestApprovalDto>> Create(CreateLeaveRequestApprovalDto dto, CancellationToken cancellationToken)
    {
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
    public async Task<IActionResult> Update(long id, UpdateLeaveRequestApprovalDto dto, CancellationToken cancellationToken)
    {
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

