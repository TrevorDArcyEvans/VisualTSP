namespace VisualTSP.Models;

public sealed class Link
{
    public Guid Id { get; set; }
    public Guid Start { get; set; }
    public Guid End { get; set; }
    public int Cost { get; set; }
}