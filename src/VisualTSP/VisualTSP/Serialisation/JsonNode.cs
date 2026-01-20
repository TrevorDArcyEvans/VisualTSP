namespace VisualTSP.Serialisation;

using Models;

public sealed class JsonNode
{
    public int Top { get; set; }
    public int Left { get; set; }

    public Node Node { get; set; }

    public JsonNode(VisualNode node)
    {
        Node = node.Node;

        Top = (int) Canvas.GetTop(node);
        Left = (int) Canvas.GetLeft(node);
    }

    // JSON deserialisation constructor
    public JsonNode()
    {
    }
}
