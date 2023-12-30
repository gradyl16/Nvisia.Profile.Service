namespace Nvisia.Profile.Service.DTO.Models;

/// <summary>
/// An Highlight entry.
/// This is the Data Transfer Object (DTO) used to communicate with the Api consumers. We do not want to expose the entity classes.
/// We are using the AutoMapper Package to map between DTO and Entity Objects. The member names should match between the two.
/// </summary>
public class HighlightDTO
{
    public int? HighlightId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ProfileId { get; set; }
}