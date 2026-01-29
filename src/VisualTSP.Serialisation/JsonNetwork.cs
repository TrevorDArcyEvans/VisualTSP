using VisualTSP.Models;

namespace VisualTSP.Serialisation;

public sealed class JsonNetwork
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public List<JsonNode> Nodes { get; set; }
    public List<JsonLink> Links { get; set; }

    public Guid Start { get; set; }
    public Guid End { get; set; }

    public JsonNetwork(Guid start, Guid end)
    {
        Start = start;
        End = end;
    }

    // JSON deserialisation constructor
    public JsonNetwork()
    {
    }

    public Network ToNetwork()
    {
        return new Network
        {
            Id = Id,
            Name = Name,
            Nodes = Nodes.Select(x => x.Node).ToList(),
            Links = Links.Select(x => x.Link).ToList() ,
            Start = Start,
            End = End,
        };
    }
}
