using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

public class EducationRepositoryTest
{
    private static readonly Fixture Fixture = new();
    
    [Test]
    public async Task TestWriteEducations_Empty()
    {
        const string dbName = "Write_Educations_Empty";
        ICollection<EducationEntity> educationEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var educations = Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var educationRepository = new EducationRepository(context);
            educationEntities = await educationRepository.WriteEducations(educations);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var educationIds = educationEntities.Select(x => x.EducationId).ToList();
            var educations =
                await context.Educations.Where(
                    x => educationIds.Contains(x.EducationId)).ToListAsync();

            var results = educationEntities.ToList();
            educations.Should().NotBeNull();
            educations[0].EducationId.Should().Be(results[0].EducationId);
            educations[0].SchoolName.Should().Be(results[0].SchoolName);
            educations[0].GraduationYear.Should().Be(results[0].GraduationYear);
            educations[0].MajorDegreeName.Should().Be(results[0].MajorDegreeName);
            educations[0].MinorDegreeName.Should().Be(results[0].MinorDegreeName);
            educations[0].ProfileId.Should().Be(results[0].ProfileId);

            educations[1].EducationId.Should().Be(results[1].EducationId);
            educations[1].SchoolName.Should().Be(results[1].SchoolName);
            educations[1].GraduationYear.Should().Be(results[1].GraduationYear);
            educations[1].MajorDegreeName.Should().Be(results[1].MajorDegreeName);
            educations[1].MinorDegreeName.Should().Be(results[1].MinorDegreeName);
            educations[1].ProfileId.Should().Be(results[1].ProfileId);

            educations[2].EducationId.Should().Be(results[2].EducationId);
            educations[2].SchoolName.Should().Be(results[2].SchoolName);
            educations[2].GraduationYear.Should().Be(results[2].GraduationYear);
            educations[2].MajorDegreeName.Should().Be(results[2].MajorDegreeName);
            educations[2].MinorDegreeName.Should().Be(results[2].MinorDegreeName);
            educations[2].ProfileId.Should().Be(results[2].ProfileId);
        }
    }

    [Test]
    public async Task TestWriteEducations_HasRecords()
    {
        const string dbName = "Write_Educations_HasRecords";
        ICollection<EducationEntity> existingEducationEntities;
        ICollection<EducationEntity> educationEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var educations = Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var educationRepository = new EducationRepository(context);
            existingEducationEntities = await educationRepository.WriteEducations(educations);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var educations = Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var educationRepository = new EducationRepository(context);
            educationEntities = await educationRepository.WriteEducations(educations);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var educationIds = educationEntities.Select(x => x.EducationId).ToList();
            var educations =
                await context.Educations.Where(
                    x => educationIds.Contains(x.EducationId)).OrderBy(x => x.EducationId).ToListAsync();

            var results = educationEntities.OrderBy(x => x.EducationId).ToList();
            educations.Should().NotBeNull();
            educations[0].EducationId.Should().Be(results[0].EducationId);
            educations[0].SchoolName.Should().Be(results[0].SchoolName);
            educations[0].GraduationYear.Should().Be(results[0].GraduationYear);
            educations[0].MajorDegreeName.Should().Be(results[0].MajorDegreeName);
            educations[0].MinorDegreeName.Should().Be(results[0].MinorDegreeName);
            educations[0].ProfileId.Should().Be(results[0].ProfileId);

            educations[1].EducationId.Should().Be(results[1].EducationId);
            educations[1].SchoolName.Should().Be(results[1].SchoolName);
            educations[1].GraduationYear.Should().Be(results[1].GraduationYear);
            educations[1].MajorDegreeName.Should().Be(results[1].MajorDegreeName);
            educations[1].MinorDegreeName.Should().Be(results[1].MinorDegreeName);
            educations[1].ProfileId.Should().Be(results[1].ProfileId);

            educations[2].EducationId.Should().Be(results[2].EducationId);
            educations[2].SchoolName.Should().Be(results[2].SchoolName);
            educations[2].GraduationYear.Should().Be(results[2].GraduationYear);
            educations[2].MajorDegreeName.Should().Be(results[2].MajorDegreeName);
            educations[2].MinorDegreeName.Should().Be(results[2].MinorDegreeName);
            educations[2].ProfileId.Should().Be(results[2].ProfileId);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var existingEducationIds = existingEducationEntities.Select(x => x.EducationId).ToList();
            var educations =
                await context.Educations.Where(
                    x => existingEducationIds.Contains(x.EducationId)).ToListAsync();

            educations.Should().BeEmpty();
        }
    }
}