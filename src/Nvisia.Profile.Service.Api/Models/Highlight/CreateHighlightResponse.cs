namespace Nvisia.Profile.Service.Api.Models.Highlight;

public class CreateHighlightResponse
{
    public int HighlightId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string Description { get; set; } = null!;
}