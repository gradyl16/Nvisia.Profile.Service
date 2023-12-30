using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

/// <summary>
/// Test the TitleCode Repository using the in-memory database. <b>The in-memory cannot be used to test sql queries, only linq queries</b>.
/// These unit tests are to validate our entities and the linq queries. SQL Query testing should be done in integration with the target DB.
/// We create a new context between inserting the data to make sure it's persisted, and there are no side-effects.
/// </summary>
public class TitleCodeRepositoryTest
{
    private static readonly Fixture Fixture = new();
    
    [Test]
    public async Task TestGetTitleCodes()
    {
        const string dbName = "Get_TitleCodes";
        var titleCodeEntities = Fixture.CreateMany<TitleCodeEntity>(3).ToList();
        await using (var context = TestUtils.CreateContext(dbName))
        {
            await context.AddRangeAsync(titleCodeEntities);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var titleCodeRepository = new TitleCodeRepository(context);
            var result = await titleCodeRepository.GetTitleCodes();
            
            var titleCodes = result.ToList();
            titleCodes.Should().NotBeNull();
            titleCodes.Should().NotBeEmpty();
            titleCodes[0].Code.Should().Be(titleCodeEntities[0].Code);
            titleCodes[0].Description.Should().Be(titleCodeEntities[0].Description);
            
            titleCodes[1].Code.Should().Be(titleCodeEntities[1].Code);
            titleCodes[1].Description.Should().Be(titleCodeEntities[1].Description);
            
            titleCodes[2].Code.Should().Be(titleCodeEntities[2].Code);
            titleCodes[2].Description.Should().Be(titleCodeEntities[2].Description);
        }
    }
    
}