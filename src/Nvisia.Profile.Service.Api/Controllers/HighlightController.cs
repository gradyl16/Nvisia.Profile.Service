using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.Highlight.BaseRoute)]
public class HighlightController : ControllerBase
{
    private readonly IHighlightService _highlightService;
    private readonly IValidator<BatchHighlightRequest> _batchCreateHighlightRequestValidator;
    private readonly IMapper _mapper;

    public HighlightController(IHighlightService highlightService,
        IValidator<BatchHighlightRequest> batchCreateHighlightRequestValidator,
        IMapper mapper)
    {
        _highlightService = highlightService ?? throw new ArgumentNullException(nameof(highlightService));
        _batchCreateHighlightRequestValidator = batchCreateHighlightRequestValidator ?? throw new ArgumentNullException(nameof(batchCreateHighlightRequestValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpPut(ControllerConstants.BatchRoute)]
    [ProducesResponseType(typeof(ICollection<CreateHighlightResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchHighlights([FromBody] BatchHighlightRequest request)
    {
        var validationResult = await _batchCreateHighlightRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var highlightDTOs = _mapper.Map<ICollection<HighlightDTO>>(request.Highlights);
        var createdHighlights =
            await _highlightService.BatchHighlights(request.ProfileId, highlightDTOs);
        
        if (createdHighlights.Count is 0)
            return BadRequest();

        var createdHighlightResponses = _mapper.Map<ICollection<CreateHighlightResponse>>(createdHighlights);
       
        return Accepted(createdHighlightResponses);
    }
}