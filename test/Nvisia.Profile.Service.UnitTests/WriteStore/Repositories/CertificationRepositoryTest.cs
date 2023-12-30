using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

public class CertificationRepositoryTest
{
    private static readonly Fixture Fixture = new();
    
    [Test]
    public async Task TestWriteCertifications_Empty()
    {
        const string dbName = "Write_Certifications_Empty";
        ICollection<CertificationEntity> certificationEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certifications = Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var certificationRepository = new CertificationRepository(context);
            certificationEntities = await certificationRepository.WriteCertifications(certifications);
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certificationIds = certificationEntities.Select(x => x.CertificationId).ToList();
            var certifications =
                await context.Certifications.Where(
                    x => certificationIds.Contains(x.CertificationId)).ToListAsync();

            var results = certificationEntities.ToList();
            certifications.Should().NotBeNull();
            certifications[0].CertificationId.Should().Be(results[0].CertificationId);
            certifications[0].Title.Should().Be(results[0].Title);
            certifications[0].Year.Should().Be(results[0].Year);
            certifications[0].ProfileId.Should().Be(results[0].ProfileId);
            
            certifications[1].CertificationId.Should().Be(results[1].CertificationId);
            certifications[1].Title.Should().Be(results[1].Title);
            certifications[1].Year.Should().Be(results[1].Year);
            certifications[1].ProfileId.Should().Be(results[1].ProfileId);
            
            certifications[2].CertificationId.Should().Be(results[2].CertificationId);
            certifications[2].Title.Should().Be(results[2].Title);
            certifications[2].Year.Should().Be(results[2].Year);
            certifications[2].ProfileId.Should().Be(results[2].ProfileId);
        }
    }
    
    [Test]
    public async Task TestWriteCertifications_HasRecords()
    {
        const string dbName = "Write_Certifications_HasRecords";
        ICollection<CertificationEntity> existingCertificationEntities;
        ICollection<CertificationEntity> certificationEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certifications = Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var certificationRepository = new CertificationRepository(context);
            existingCertificationEntities = await certificationRepository.WriteCertifications(certifications);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certifications = Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var certificationRepository = new CertificationRepository(context);
            certificationEntities = await certificationRepository.WriteCertifications(certifications);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certificationIds = certificationEntities.Select(x => x.CertificationId).ToList();
            var certifications =
                await context.Certifications.Where(
                    x => certificationIds.Contains(x.CertificationId)).OrderBy(x => x.CertificationId).ToListAsync();

            var results = certificationEntities.OrderBy(x => x.CertificationId).ToList();
            certifications.Should().NotBeNull();
            certifications[0].CertificationId.Should().Be(results[0].CertificationId);
            certifications[0].Title.Should().Be(results[0].Title);
            certifications[0].Year.Should().Be(results[0].Year);
            certifications[0].ProfileId.Should().Be(results[0].ProfileId);
            
            certifications[1].CertificationId.Should().Be(results[1].CertificationId);
            certifications[1].Title.Should().Be(results[1].Title);
            certifications[1].Year.Should().Be(results[1].Year);
            certifications[1].ProfileId.Should().Be(results[1].ProfileId);
            
            certifications[2].CertificationId.Should().Be(results[2].CertificationId);
            certifications[2].Title.Should().Be(results[2].Title);
            certifications[2].Year.Should().Be(results[2].Year);
            certifications[2].ProfileId.Should().Be(results[2].ProfileId);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var existingCertificationIds = existingCertificationEntities.Select(x => x.CertificationId).ToList();
            var certifications =
                await context.Certifications.Where(
                    x => existingCertificationIds.Contains(x.CertificationId)).ToListAsync();

            certifications.Should().BeEmpty();
        }
    }

    [Test]
    public async Task TestDeleteCertifications_Empty()
    {
        const string dbName = "Delete_Certifications_Empty";
        const int profileId = 1;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certificationRepository = new CertificationRepository(context);
            var result = await certificationRepository.DeleteCertificationsByProfileId(profileId);
            result.Should().BeTrue();
        }
    }

    [Test]
    public async Task TestDeleteCertifications_Existing()
    {
        const string dbName = "Delete_Certifications_Existing";
        const int profileId = 1;
        ICollection<CertificationEntity> certificationEntities;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profile);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certifications = Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity)
                .With(x => x.ProfileId, 1).CreateMany(3).ToList();
            var certificationRepository = new CertificationRepository(context);
            certificationEntities = await certificationRepository.WriteCertifications(certifications);
        }

        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certificationRepository = new CertificationRepository(context);
            var result = await certificationRepository.DeleteCertificationsByProfileId(profileId);
            result.Should().BeTrue();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var certificationIds = certificationEntities.Select(x => x.CertificationId).ToList();
            var certifications = await context.Certifications.Where(x => certificationIds.Contains(x.CertificationId)).ToListAsync();
            certifications.Should().BeEmpty();
        }
    }
}