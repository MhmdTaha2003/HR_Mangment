using FluentValidation;
using HR.Application.DTOs.DocumentTypes;
using HR.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;
[ApiController, Route("api/[controller]")]
public class DocumentTypesController : ControllerBase
{
    private readonly IDocumentTypeService _documentTypeService;
    private readonly IValidator<CreateDocumentTypeDto> _createValidator;
    private readonly IValidator<UpdateDocumentTypeDto> _updateValidator;

    public DocumentTypesController(
        IDocumentTypeService documentTypeService,
        IValidator<CreateDocumentTypeDto> createValidator,
        IValidator<UpdateDocumentTypeDto> updateValidator)
    {
        _documentTypeService = documentTypeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentTypeDto>>> GetAll(CancellationToken cancellationToken) => Ok(await _documentTypeService.GetAllAsync(cancellationToken));
    [HttpGet("{id:long}")]
    public async Task<ActionResult<DocumentTypeDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _documentTypeService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DocumentTypeDto>> Create(CreateDocumentTypeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        try
        {
            var result = await _documentTypeService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateDocumentTypeDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        return await _documentTypeService.UpdateAsync(id, dto, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken) => await _documentTypeService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}

