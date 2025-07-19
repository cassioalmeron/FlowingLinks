namespace FlowingLinks.Core.Models;

public record Link : EntityBase
{
    public string Description { get; set; }
    public string Url { get; set; }
    public string? Comments { get; set; }
    public bool Read { get; set; } = false;
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<LinkLabel> LinkLabels { get; set; } = new List<LinkLabel>();
}