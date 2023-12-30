using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

public class HighlightRepositoryTest
{
    private static readonly Fixture Fixture = new();
    
    [Test]
    public async Task TestWriteHighlights_Empty()
    {
        const string dbName = "Write_Highlights_Empty";
        ICollection<HighlightEntity> highlightEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var highlights = Fixture.Build<HighlightEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .CreateMany(3).ToList();
            var highlightRepository = new HighlightRepository(context);
            highlightEntities = await highlightRepository.WriteHighlights(highlights);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var highlightIds = highlightEntities.Select(x => x.HighlightId).ToList();
            var highlights =
                await context.Highlights.Where(
                    x => highlightIds.Contains(x.HighlightId)).ToListAsync();

            var results = highlightEntities.ToList();
            highlights.Should().NotBeNull();
            highlights[0].HighlightId.Should().Be(results[0].HighlightId);
            highlights[0].Title.Should().Be(results[0].Title);
            highlights[0].Description.Should().Be(results[0].Description);
            highlights[0].ProfileId.Should().Be(results[0].ProfileId);

            highlights[1].HighlightId.Should().Be(results[1].HighlightId);
            highlights[1].Title.Should().Be(results[1].Title);
            highlights[1].Description.Should().Be(results[1].Description);
            highlights[1].ProfileId.Should().Be(results[1].ProfileId);

            highlights[2].HighlightId.Should().Be(results[2].HighlightId);
            highlights[2].Title.Should().Be(results[2].Title);
            highlights[2].Description.Should().Be(results[2].Description);
            highlights[2].ProfileId.Should().Be(results[2].ProfileId);
        }
    }

    [Test]
    public async Task TestWriteHighlights_HasRecords()
    {
        const string dbName = "Write_Highlights_HasRecords";
        ICollection<HighlightEntity> existingHighlightEntities;
        ICollection<HighlightEntity> highlightEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var highlights = Fixture.Build<HighlightEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .CreateMany(3).ToList();
            var highlightRepository = new HighlightRepository(context);
            existingHighlightEntities = await highlightRepository.WriteHighlights(highlights);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var highlights = Fixture.Build<HighlightEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .CreateMany(3).ToList();
            var highlightRepository = new HighlightRepository(context);
            highlightEntities = await highlightRepository.WriteHighlights(highlights);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var highlightIds = highlightEntities.Select(x => x.HighlightId).ToList();
            var highlights =
                await context.Highlights.Where(
                    x => highlightIds.Contains(x.HighlightId)).OrderBy(x => x.HighlightId).ToListAsync();

            var results = highlightEntities.OrderBy(x => x.HighlightId).ToList();
            highlights.Should().NotBeNull();
            highlights[0].HighlightId.Should().Be(results[0].HighlightId);
            highlights[0].Title.Should().Be(results[0].Title);
            highlights[0].Description.Should().Be(results[0].Description);
            highlights[0].ProfileId.Should().Be(results[0].ProfileId);

            highlights[1].HighlightId.Should().Be(results[1].HighlightId);
            highlights[1].Title.Should().Be(results[1].Title);
            highlights[1].Description.Should().Be(results[1].Description);
            highlights[1].ProfileId.Should().Be(results[1].ProfileId);

            highlights[2].HighlightId.Should().Be(results[2].HighlightId);
            highlights[2].Title.Should().Be(results[2].Title);
            highlights[2].Description.Should().Be(results[2].Description);
            highlights[2].ProfileId.Should().Be(results[2].ProfileId);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var existingHighlightIds = existingHighlightEntities.Select(x => x.HighlightId).ToList();
            var highlights =
                await context.Highlights.Where(
                    x => existingHighlightIds.Contains(x.HighlightId)).ToListAsync();

            highlights.Should().BeEmpty();
        }
    }
}