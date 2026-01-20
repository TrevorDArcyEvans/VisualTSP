namespace VisualTSP.Serialisation;

public sealed class JsonNetwork
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<JsonNode> Nodes { get; set; }
    public List<JsonLink> Links { get; set; }

    public JsonNode Start { get; set; }
    public JsonNode End { get; set; }

    public JsonNetwork(JsonNode start, JsonNode end)
    {
        Start = start;
        End = end;
    }

    // JSON deserialisation constructor
    public JsonNetwork()
    {
    }
}
