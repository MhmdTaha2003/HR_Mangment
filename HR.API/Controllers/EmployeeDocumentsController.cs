using FluentValidation;
using HR.Application.Common.Security;
using HR.API.Authorization;
using HR.Application.DTOs.EmployeeDocuments;
using HR.Application.Interfaces.Authentication;
using HR.Application.Interfaces;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
[Authorize]
public class EmployeeDocumentsController : ControllerBase
{
    private readonly IEmployeeDocumentService _employeeDocumentService;
    private readonly IValidator<CreateEmployeeDocumentDto> _createValidator;
    private readonly IValidator<UpdateEmployeeDocumentDto> _updateValidator;

    public EmployeeDocumentsController(
        IEmployeeDocumentService employeeDocumentService,
        IValidator<CreateEmployeeDocumentDto> createValidator,
        IValidator<UpdateEmployeeDocumentDto> updateValidator)
    {
        _employeeDocumentService = employeeDocumentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<List<EmployeeDocumentDto>>> GetAll([FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var records = await _employeeDocumentService.GetAllAsync(cancellationToken);
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(records);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return Ok(records.Where(record => ids.Contains(record.EmployeeId)).ToList());
    }
    [HttpGet("{id:long}")]
    [Authorize(Policy = AppPolicies.LeaveApproval)]
    public async Task<ActionResult<EmployeeDocumentDto>> GetById(long id, [FromServices] ICurrentUserService currentUser, [FromServices] IManagerTeamService teams, CancellationToken cancellationToken)
    {
        var result = await _employeeDocumentService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        if (ManagerTeamAccess.HasFullAccess(User)) return Ok(result);
        var ids = await ManagerTeamAccess.GetTeamIdsAsync(currentUser, teams, cancellationToken);
        return ids.Contains(result.EmployeeId) ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<ActionResult<EmployeeDocumentDto>> Create(CreateEmployeeDocumentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _employeeDocumentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = AppPolicies.HRManagement)]
    public async Task<IActionResult> Update(long id, UpdateEmployeeDocumentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            return await _employeeDocumentService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
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

        return Ok(await _employeeDocumentService.GetByEmployeeIdAsync(
            employeeId.Value,
            cancellationToken));
    }
}

