using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.Certification.BaseRoute)]
public class CertificationController : ControllerBase
{
    private readonly ICertificationService _certificationService;
    private readonly IValidator<BatchCertificationRequest> _batchCertificationRequestValidator;
    private readonly IMapper _mapper;

    public CertificationController(ICertificationService certificationService,
        IValidator<BatchCertificationRequest> batchCertificationRequestValidator,
        IMapper mapper)
    {
        _certificationService = certificationService ?? throw new ArgumentNullException(nameof(certificationService));
        _batchCertificationRequestValidator = batchCertificationRequestValidator ??
                                                    throw new ArgumentNullException(
                                                        nameof(batchCertificationRequestValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPut(ControllerConstants.BatchRoute)]
    [ProducesResponseType(typeof(ICollection<CreateCertificationResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchCertifications([FromBody] BatchCertificationRequest request)
    {
        var validationResult = await _batchCertificationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var certificationDTOs = _mapper.Map<ICollection<CertificationDTO>>(request.Certifications);
        var createdCertifications =
            await _certificationService.BatchCertifications(request.ProfileId, certificationDTOs);

        if (createdCertifications is null)
            return BadRequest();

        var createdCertificationResponses =
            _mapper.Map<ICollection<CreateCertificationResponse>>(createdCertifications);
        
        return Accepted(createdCertificationResponses);
    }
}