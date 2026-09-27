using FluentValidation;
using HR.Application.DTOs.EmployeeLeaveBalances;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
public class EmployeeLeaveBalancesController : ControllerBase
{
    private readonly IEmployeeLeaveBalanceService _employeeLeaveBalanceService;
    private readonly IValidator<CreateEmployeeLeaveBalanceDto> _createValidator;
    private readonly IValidator<UpdateEmployeeLeaveBalanceDto> _updateValidator;

    public EmployeeLeaveBalancesController(
        IEmployeeLeaveBalanceService employeeLeaveBalanceService,
        IValidator<CreateEmployeeLeaveBalanceDto> createValidator,
        IValidator<UpdateEmployeeLeaveBalanceDto> updateValidator)
    {
        _employeeLeaveBalanceService = employeeLeaveBalanceService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeLeaveBalanceDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _employeeLeaveBalanceService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EmployeeLeaveBalanceDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _employeeLeaveBalanceService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeLeaveBalanceDto>> Create(CreateEmployeeLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _employeeLeaveBalanceService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateEmployeeLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _employeeLeaveBalanceService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }
}

