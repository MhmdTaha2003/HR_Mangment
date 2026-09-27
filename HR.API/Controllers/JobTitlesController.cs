using FluentValidation;
using HR.Application.DTOs.JobTitles;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
public class JobTitlesController : ControllerBase
{
    private readonly IJobTitleService _jobTitleService;
    private readonly IValidator<CreateJobTitleDto> _createValidator;
    private readonly IValidator<UpdateJobTitleDto> _updateValidator;

    public JobTitlesController(
        IJobTitleService jobTitleService,
        IValidator<CreateJobTitleDto> createValidator,
        IValidator<UpdateJobTitleDto> updateValidator)
    {
        _jobTitleService = jobTitleService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobTitleDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _jobTitleService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<JobTitleDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _jobTitleService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<JobTitleDto>> Create(CreateJobTitleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _jobTitleService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateJobTitleDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _jobTitleService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken) => await _jobTitleService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}

