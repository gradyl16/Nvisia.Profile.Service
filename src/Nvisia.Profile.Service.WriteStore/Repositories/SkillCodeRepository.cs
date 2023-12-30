using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class SkillCodeRepository : ISkillCodeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SkillCodeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns All skill codes, no children, READ-ONLY entities
    /// </summary>
    /// <returns>List of Titles</returns>
    public async Task<ICollection<SkillCodeEntity>> GetSkillCodes()
        => await _dbContext.SkillCodes.AsNoTracking().ToListAsync();
}