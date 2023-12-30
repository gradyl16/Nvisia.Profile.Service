namespace Nvisia.Profile.Service.Api.Models.TitleCode;

public class TitleCodeResponse
{
    public int TitleCodeId { get; set; }
    
    public string Code { get; set; } = null!;
    
    public string Description { get; set; } = null!;
}