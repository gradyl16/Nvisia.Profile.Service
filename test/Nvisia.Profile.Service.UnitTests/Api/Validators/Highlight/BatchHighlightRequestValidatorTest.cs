using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Api.Validators.Highlight;

namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Highlight;

public class BatchHighlightRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly BatchHighlightRequestValidator _validator = new();
    
    [TestCase(1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_ProfileId(int profileId)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ProfileId = profileId;

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase("Title", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_Title(string title)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Highlights = CreateHighlights(title: title);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase("Description", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_Description(string description)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Highlights = CreateHighlights(description: description);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }

    [TestCase(0, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(3, ExpectedResult = true)]
    [TestCase(4, ExpectedResult = false)]
    [TestCase(10, ExpectedResult = false)]
    public async Task<bool> TestValidate_HighlightsCount(int count)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Highlights = CreateHighlights(count: count);
        
        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }

    [Test]
    public async Task TestValidate_HighlightsNull()
    {
        // Arrange
        var request = Fixture.Build<BatchHighlightRequest>()
            .With(x => x.ProfileId, 1)
            .Without(x => x.Highlights)
            .Create();

        var validationResult = await _validator.ValidateAsync(request);
        validationResult.IsValid.Should().BeFalse();
    }
    
    [Test]
    public void TestValidate_Valid()
    {
        // Arrange
        var request = CreateValidRequest();
        
        var validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    private static BatchHighlightRequest CreateValidRequest()
        => Fixture.Build<BatchHighlightRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.Highlights, CreateHighlights())
            .Create();

    private static List<HighlightRequest> CreateHighlights(string title = "Title", string description = "Description", int count = 2)
        => Fixture.Build<HighlightRequest>()
            .With(x => x.Title, title)
            .With(x => x.Description, description)
            .CreateMany(count).ToList();
}