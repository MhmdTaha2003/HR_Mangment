using FluentValidation;
using HR.Application.Common.Security;
using HR.API.Authorization;
using HR.Application.DTOs.LeaveRequests;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize]
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
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll([FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var records = await _leaveRequestService.GetAllAsync(cancellationToken);
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(records);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return Ok(records.Where(record => ids.Contains(record.EmployeeId)).ToList());
    }
    [HttpGet("{id:long}")]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<LeaveRequestDto>> GetById(long id, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(result);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return ids.Contains(result.EmployeeId) ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.HR + "," + AppRoles.Employee)]
    public async Task<ActionResult<LeaveRequestDto>> Create(
        CreateLeaveRequestDto dto,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var effectiveDto = dto;

        if (!User.IsInRole(AppRoles.Admin) && !User.IsInRole(AppRoles.HR))
        {
            var employeeId = await currentUserService.GetEmployeeIdAsync();

            if (employeeId is null)
            {
                return NotFound(new
                {
                    message = "The current user is not linked to an employee."
                });
            }

            effectiveDto = dto with { EmployeeId = employeeId.Value };
        }

        var validationResult = await _createValidator.ValidateAsync(effectiveDto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _leaveRequestService.CreateAsync(effectiveDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
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

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken)
    {
        var employeeId = await currentUserService.GetEmployeeIdAsync();

        if (employeeId is null)
        {
            return NotFound(new
            {
                message = "The current user is not linked to an employee."
            });
        }

        return Ok(await _leaveRequestService.GetByEmployeeIdAsync(
            employeeId.Value,
            cancellationToken));
    }
}

