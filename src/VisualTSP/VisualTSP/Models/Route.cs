namespace VisualTSP.Models;

public sealed class Route
{
    public Guid Id { get; set; }
    public List<Node> Nodes { get; set; } = [];
}