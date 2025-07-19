namespace FlowingLinks.Core.Models;

public record LinkLabel : EntityBase
{
    public int LinkId { get; set; }
    public Link Link { get; set; }
    public int LabelId { get; set; }
    public Label Label { get; set; }
}