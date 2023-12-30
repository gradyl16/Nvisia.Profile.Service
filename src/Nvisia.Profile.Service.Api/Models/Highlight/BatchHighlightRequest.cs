namespace Nvisia.Profile.Service.Api.Models.Highlight;

public class BatchHighlightRequest
{
    public int ProfileId { get; set; }
    
    public ICollection<HighlightRequest> Highlights { get; set; } = new List<HighlightRequest>();
}