using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface IHighlightService
{
    Task<ICollection<HighlightDTO>> BatchHighlights(int profileId, ICollection<HighlightDTO> educations);

}