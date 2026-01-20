namespace VisualTSP.Models;

public sealed class Link
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid Start { get; set; } = Guid.NewGuid();
    public Guid End { get; set; } = Guid.NewGuid();
    public int Cost { get; set; }
}
