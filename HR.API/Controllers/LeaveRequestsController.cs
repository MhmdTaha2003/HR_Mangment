using FluentValidation;
using HR.Application.Common.Security;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize(Policy = AppPolicies.HRManagement)]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    private readonly IValidator<CreateLeaveRequestDto> _createValidator;
    private readonly IValidator<UpdateLeaveRequestDto> _updateValidator;

    public LeaveRequestsController(
        ILeaveRequestService leaveRequestService,
        IValidator<CreateLeaveRequestDto> createValidator,
        IValidator<UpdateLeaveRequestDto> updateValidator)
    {
        _leaveRequestService = leaveRequestService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _leaveRequestService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<LeaveRequestDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(CreateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _leaveRequestService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            return await _leaveRequestService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}

