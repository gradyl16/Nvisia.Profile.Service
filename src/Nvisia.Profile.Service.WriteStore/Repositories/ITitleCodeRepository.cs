using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface ITitleCodeRepository
{
    Task<ICollection<TitleCodeEntity>> GetTitleCodes();
}