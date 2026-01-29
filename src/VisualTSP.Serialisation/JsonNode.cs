namespace VisualTSP.Serialisation;

using Models;

public sealed class JsonNode
{
    public int Top { get; set; }
    public int Left { get; set; }

    public Node Node { get; set; }

    // JSON deserialisation constructor
    public JsonNode()
    {
    }
}
