using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

public class SkillRepositoryTest
{
    private static readonly Fixture Fixture = new();
    
    [Test]
    public async Task TestWriteSkills_Empty()
    {
        const string dbName = "Write_Skills_Empty";
        ICollection<SkillEntity> skillEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        var skillCodeEntity = Fixture.Create<SkillCodeEntity>();
        await using (var context = TestUtils.CreateContext(dbName))
        {
            await context.AddAsync(skillCodeEntity);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skills = Fixture.Build<SkillEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .Without(x => x.SkillCode)
                .With(x => x.SkillCodeId, skillCodeEntity.SkillCodeId)
                .CreateMany(3).ToList();
            var skillRepository = new SkillRepository(context);
            skillEntities = await skillRepository.WriteSkills(skills);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skillIds = skillEntities.Select(x => x.SkillId).ToList();
            var skills =
                await context.Skills.Where(
                    x => skillIds.Contains(x.SkillId)).ToListAsync();

            var results = skillEntities.ToList();
            skills.Should().NotBeNull();
            skills[0].SkillId.Should().Be(results[0].SkillId);
            skills[0].SkillCodeId.Should().Be(results[0].SkillCodeId);
            skills[0].Description.Should().Be(results[0].Description);
            skills[0].SortOrder.Should().Be(results[0].SortOrder);
            skills[0].ProfileId.Should().Be(results[0].ProfileId);

            skills[1].SkillId.Should().Be(results[1].SkillId);
            skills[1].SkillCodeId.Should().Be(results[1].SkillCodeId);
            skills[1].Description.Should().Be(results[1].Description);
            skills[1].SortOrder.Should().Be(results[1].SortOrder);
            skills[1].ProfileId.Should().Be(results[1].ProfileId);

            skills[2].SkillId.Should().Be(results[2].SkillId);
            skills[2].SkillCodeId.Should().Be(results[2].SkillCodeId);
            skills[2].Description.Should().Be(results[2].Description);
            skills[2].SortOrder.Should().Be(results[2].SortOrder);
            skills[2].ProfileId.Should().Be(results[2].ProfileId);
        }
    }

    [Test]
    public async Task TestWriteSkills_HasRecords()
    {
        const string dbName = "Write_Skills_HasRecords";
        ICollection<SkillEntity> existingSkillEntities;
        ICollection<SkillEntity> skillEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        var skillCodeEntity = Fixture.Create<SkillCodeEntity>();
        await using (var context = TestUtils.CreateContext(dbName))
        {
            await context.AddAsync(skillCodeEntity);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skills = Fixture.Build<SkillEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .Without(x => x.SkillCode)
                .With(x => x.SkillCodeId, skillCodeEntity.SkillCodeId)
                .CreateMany(3).ToList();
            var skillRepository = new SkillRepository(context);
            existingSkillEntities = await skillRepository.WriteSkills(skills);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skills = Fixture.Build<SkillEntity>()
                .Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1)
                .Without(x => x.SkillCode)
                .With(x => x.SkillCodeId, skillCodeEntity.SkillCodeId)
                .CreateMany(3).ToList();
            var skillRepository = new SkillRepository(context);
            skillEntities = await skillRepository.WriteSkills(skills);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var skillIds = skillEntities.Select(x => x.SkillId).ToList();
            var skills =
                await context.Skills.Where(
                    x => skillIds.Contains(x.SkillId)).OrderBy(x => x.SkillId).ToListAsync();

            var results = skillEntities.OrderBy(x => x.SkillId).ToList();
            skills.Should().NotBeNull();
            skills[0].SkillId.Should().Be(results[0].SkillId);
            skills[0].SkillCodeId.Should().Be(results[0].SkillCodeId);
            skills[0].Description.Should().Be(results[0].Description);
            skills[0].SortOrder.Should().Be(results[0].SortOrder);
            skills[0].ProfileId.Should().Be(results[0].ProfileId);

            skills[1].SkillId.Should().Be(results[1].SkillId);
            skills[1].SkillCodeId.Should().Be(results[1].SkillCodeId);
            skills[1].Description.Should().Be(results[1].Description);
            skills[1].SortOrder.Should().Be(results[1].SortOrder);
            skills[1].ProfileId.Should().Be(results[1].ProfileId);

            skills[2].SkillId.Should().Be(results[2].SkillId);
            skills[2].SkillCodeId.Should().Be(results[2].SkillCodeId);
            skills[2].Description.Should().Be(results[2].Description);
            skills[2].SortOrder.Should().Be(results[2].SortOrder);
            skills[2].ProfileId.Should().Be(results[2].ProfileId);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var existingSkillIds = existingSkillEntities.Select(x => x.SkillId).ToList();
            var skills =
                await context.Skills.Where(
                    x => existingSkillIds.Contains(x.SkillId)).ToListAsync();

            skills.Should().BeEmpty();
        }
    }
}