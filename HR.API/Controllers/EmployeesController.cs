using FluentValidation;
using HR.Application.Common.Security;
using HR.API.Authorization;
using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IValidator<CreateEmployeeDto> _createValidator;
    private readonly IValidator<UpdateEmployeeDto> _updateValidator;

    public EmployeesController(
        IEmployeeService employeeService,
        IValidator<CreateEmployeeDto> createValidator,
        IValidator<UpdateEmployeeDto> updateValidator)
    {
        _employeeService = employeeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll([FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var records = await _employeeService.GetAllAsync(cancellationToken);
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(records);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return Ok(records.Where(record => ids.Contains(record.Id)).ToList());
    }


    [HttpGet("{id:long}")]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<EmployeeDto>> GetById(long id, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(result);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return ids.Contains(result.Id) ? Ok(result) : NotFound();
    }


    [HttpPost]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _employeeService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<IActionResult> Update(long id, UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _employeeService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }



    [HttpGet("me")]
    public async Task<IActionResult> GetMe(
    [FromServices] ICurrentUserService currentUserService, CancellationToken cancellationToken)
    {
        var employeeId = await currentUserService.GetEmployeeIdAsync();

        if (employeeId is null)
        {
            return NotFound(new
            {
                message = "The current user is not linked to an employee."
            });
        }

        var employee = await _employeeService.GetByIdAsync(employeeId.Value , cancellationToken);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }
}

