namespace VisualTSP.Serialisation;

using Models;

public sealed class JsonLink
{
    public int X1 { get; set; }
    public int Y1 { get; set; }
    public int X2 { get; set; }
    public int Y2 { get; set; }

    public Link Link { get; set; }

    public JsonLink(VisualLink link)
    {
        Link = link.Link;

        X1 = (int) link.X1;
        Y1 = (int) link.Y1;
        X2 = (int) link.X2;
        Y2 = (int) link.Y2;
    }

    // JSON deserialisation constructor
    public JsonLink()
    {
    }
}
