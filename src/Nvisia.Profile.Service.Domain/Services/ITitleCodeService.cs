using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface ITitleCodeService
{
    Task<ICollection<TitleCodeDTO>> GetTitleCodes();
}