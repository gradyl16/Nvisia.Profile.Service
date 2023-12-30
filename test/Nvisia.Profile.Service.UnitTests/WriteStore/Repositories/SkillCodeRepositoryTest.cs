using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

/// <summary>
/// Test the SkillCode Repository using the in-memory database. <b>The in-memory cannot be used to test sql queries, only linq queries</b>.
/// These unit tests are to validate our entities and the linq queries. SQL Query testing should be done in integration with the target DB.
/// We create a new context between inserting the data to make sure it's persisted, and there are no side-effects.
/// </summary>
public class SkillCodeRepositoryTest
{
    private static readonly Fixture Fixture = new();

    [Test]
    public async Task TestGetSkillCodes()
    {
        const string dbName = "Get_SkillCodes";
        var skillCodeEntities = Fixture.CreateMany<SkillCodeEntity>(3).ToList();
        await using (var context = TestUtils.CreateContext(dbName))
        {
            await context.AddRangeAsync(skillCodeEntities);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skillCodeRepository = new SkillCodeRepository(context);
            var result = await skillCodeRepository.GetSkillCodes();
            
            var skillCodes = result.ToList();
            skillCodes.Should().NotBeNull();
            skillCodes.Should().NotBeEmpty();
            skillCodes[0].Code.Should().Be(skillCodeEntities[0].Code);
            skillCodes[0].Description.Should().Be(skillCodeEntities[0].Description);
            
            skillCodes[1].Code.Should().Be(skillCodeEntities[1].Code);
            skillCodes[1].Description.Should().Be(skillCodeEntities[1].Description);
            
            skillCodes[2].Code.Should().Be(skillCodeEntities[2].Code);
            skillCodes[2].Description.Should().Be(skillCodeEntities[2].Description);
        }
    }

}