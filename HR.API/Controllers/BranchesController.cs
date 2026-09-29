using FluentValidation;
using HR.Application.Common.Security;
using HR.Application.DTOs.Branches;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;
    private readonly IValidator<CreateBranchDto> _createValidator;
    private readonly IValidator<UpdateBranchDto> _updateValidator;
    public BranchesController(IBranchService branchService, IValidator<CreateBranchDto> createValidator, IValidator<UpdateBranchDto> updateValidator)
    {
        _branchService = branchService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BranchDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _branchService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BranchDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _branchService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<ActionResult<BranchDto>> Create(CreateBranchDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        BranchDto result;
        try
        {
            result = await _branchService.CreateAsync(dto, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<IActionResult> Update(long id, UpdateBranchDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updated = await _branchService.UpdateAsync(id, dto, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _branchService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
