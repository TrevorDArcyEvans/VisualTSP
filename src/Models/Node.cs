namespace VisualTSP.Models;

public sealed class Node
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public required string Name { get; set; }
}
