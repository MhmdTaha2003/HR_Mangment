using FluentValidation;
using HR.Application.Common.Security;
using HR.API.Authorization;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize]
public class EmployeeAssignmentsController : ControllerBase
{
    private readonly IEmployeeAssignmentService _employeeAssignmentService;
    private readonly IValidator<CreateEmployeeAssignmentDto> _createValidator;
    private readonly IValidator<UpdateEmployeeAssignmentDto> _updateValidator;

    public EmployeeAssignmentsController(
        IEmployeeAssignmentService employeeAssignmentService,
        IValidator<CreateEmployeeAssignmentDto> createValidator,
        IValidator<UpdateEmployeeAssignmentDto> updateValidator)
    {
        _employeeAssignmentService = employeeAssignmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<List<EmployeeAssignmentDto>>> GetAll([FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var records = await _employeeAssignmentService.GetAllAsync(cancellationToken);
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(records);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return Ok(records.Where(record => record.IsCurrent && ids.Contains(record.EmployeeId)).ToList());
    }
    [HttpGet("{id:long}")]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<EmployeeAssignmentDto>> GetById(long id, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var result = await _employeeAssignmentService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(result);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return result.IsCurrent && ids.Contains(result.EmployeeId) ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<ActionResult<EmployeeAssignmentDto>> Create(CreateEmployeeAssignmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _employeeAssignmentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<IActionResult> Update(long id, UpdateEmployeeAssignmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            return await _employeeAssignmentService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
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

        return Ok(await _employeeAssignmentService.GetByEmployeeIdAsync(
            employeeId.Value,
            cancellationToken));
    }
}

