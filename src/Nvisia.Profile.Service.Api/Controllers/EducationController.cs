using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.Education.BaseRoute)]
public class EducationController
    : ControllerBase
{
    private readonly IEducationService _educationService;
    private readonly IValidator<BatchEducationRequest> _batchCreateEducationRequestValidator;
    private readonly IMapper _mapper;

    public EducationController(IEducationService educationService,
        IValidator<BatchEducationRequest> batchCreateEducationRequestValidator,
        IMapper mapper)
    {
        _educationService = educationService ?? throw new ArgumentNullException(nameof(educationService));
        _batchCreateEducationRequestValidator = batchCreateEducationRequestValidator ?? throw new ArgumentNullException(nameof(batchCreateEducationRequestValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPut(ControllerConstants.BatchRoute)]
    [ProducesResponseType(typeof(ICollection<CreateEducationResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchEducations([FromBody] BatchEducationRequest request)
    {
        var validationResult = await _batchCreateEducationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var educationDTOs = _mapper.Map<ICollection<EducationDTO>>(request.Educations);
        var createdEducations =
            await _educationService.BatchEducations(request.ProfileId, educationDTOs);
        
        if (createdEducations.Count is 0)
            return BadRequest();

        var createdEducationResponses = _mapper.Map<ICollection<CreateEducationResponse>>(createdEducations);
       
        return Accepted(createdEducationResponses);
    }
}