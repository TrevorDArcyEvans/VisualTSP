namespace VisualTSP.Models;

public sealed class Network
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<Node> Nodes { get; set; } = [];
    public List<Link> Links { get; set; } = [];
    public required Node Start { get; set; }
    public required Node End { get; set; }
}