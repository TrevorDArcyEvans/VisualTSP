namespace VisualTSP.Models;

public sealed class Network
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<Node> Nodes { get; set; } = [];
    public List<Link> Links { get; set; } = [];
    public Guid Start { get; set; }
    public Guid End { get; set; }
}
