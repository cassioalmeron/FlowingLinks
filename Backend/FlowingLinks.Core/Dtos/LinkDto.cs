namespace FlowingLinks.Core.Dtos;

public class LinkDto : DtoBase
{
    public string Description { get; set; }
    public string Url { get; set; }
    public string? Comments { get; set; }
    public bool Read { get; set; } = false;
    public List<int> LabelIds { get; set; } = new ();
}