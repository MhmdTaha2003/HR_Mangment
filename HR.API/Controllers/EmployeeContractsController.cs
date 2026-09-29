using FluentValidation;
using HR.Application.Common.Security;
using HR.Application.DTOs.EmployeeContracts;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize(Policy = AppPolicies.HRManagement)]
public class EmployeeContractsController : ControllerBase
{
    private readonly IEmployeeContractService _employeeContractService;
    private readonly IValidator<CreateEmployeeContractDto> _createValidator;
    private readonly IValidator<UpdateEmployeeContractDto> _updateValidator;

    public EmployeeContractsController(
        IEmployeeContractService employeeContractService,
        IValidator<CreateEmployeeContractDto> createValidator,
        IValidator<UpdateEmployeeContractDto> updateValidator)
    {
        _employeeContractService = employeeContractService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeContractDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _employeeContractService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<EmployeeContractDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _employeeContractService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeContractDto>> Create(CreateEmployeeContractDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _employeeContractService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateEmployeeContractDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _employeeContractService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }
}

