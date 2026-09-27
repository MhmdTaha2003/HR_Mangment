using FluentValidation;
using HR.Application.DTOs.LeaveTypes;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
public class LeaveTypesController : ControllerBase
{
    private readonly ILeaveTypeService _leaveTypeService;
    private readonly IValidator<CreateLeaveTypeDto> _createValidator;
    private readonly IValidator<UpdateLeaveTypeDto> _updateValidator;

    public LeaveTypesController(
        ILeaveTypeService leaveTypeService,
        IValidator<CreateLeaveTypeDto> createValidator,
        IValidator<UpdateLeaveTypeDto> updateValidator)
    {
        _leaveTypeService = leaveTypeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaveTypeDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _leaveTypeService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<LeaveTypeDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _leaveTypeService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveTypeDto>> Create(CreateLeaveTypeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _leaveTypeService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateLeaveTypeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _leaveTypeService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken) => await _leaveTypeService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}

