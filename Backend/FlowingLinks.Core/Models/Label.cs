namespace FlowingLinks.Core.Models
{
    public record Label : EntityBase
    {
        public string Name { get; set; }
        public ICollection<LinkLabel> LinkLabels { get; set; } = new List<LinkLabel>();
    }
}