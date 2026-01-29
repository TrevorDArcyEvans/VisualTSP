namespace VisualTSP.Serialisation;

using Models;

public sealed class JsonLink
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }

    public Link Link { get; set; }

    // JSON deserialisation constructor
    public JsonLink()
    {
    }
}
