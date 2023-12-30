using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SkillRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Insert multiple skills
    /// </summary>
    /// <param name="skills"></param>
    /// <returns>Collection of skills</returns>
    public async Task<ICollection<SkillEntity>> WriteSkills(ICollection<SkillEntity> skills)
    {
        var existingSkills = await GetExistingSkills(skills.First().ProfileId);

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        _dbContext.Skills.RemoveRange(existingSkills);
        await _dbContext.SaveChangesAsync();

        await _dbContext.AddRangeAsync(skills);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return skills;
    }

    private async Task<ICollection<SkillEntity>> GetExistingSkills(int profileId) 
        => await _dbContext.Skills.Where(x => x.ProfileId == profileId).ToListAsync();
}