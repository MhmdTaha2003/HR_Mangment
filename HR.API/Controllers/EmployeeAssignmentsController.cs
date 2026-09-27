using FluentValidation;
using HR.Application.DTOs.EmployeeAssignments;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
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
    public async Task<ActionResult<List<EmployeeAssignmentDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _employeeAssignmentService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EmployeeAssignmentDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _employeeAssignmentService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
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
}

