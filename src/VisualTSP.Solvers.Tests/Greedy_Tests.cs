namespace VisualTSP.Solvers.Tests;

using Models;
using Newtonsoft.Json;
using Serialisation;
using Shouldly;

[TestFixture]
public sealed class Greedy_Tests
{
    private Network _network;
    private List<Link> _route;

    [SetUp]
    public void Setup()
    {
        var filePath = Path.Combine("samples", "New Document.tsp");
        var json = File.ReadAllText(filePath);
        var jsonNetwork = JsonConvert.DeserializeObject<JsonNetwork>(json);
        _network = jsonNetwork.ToNetwork();
        var solver = new Greedy(_network);
        _route = solver.Solve();
    }

    [Test]
    public void Solve_returns_expected_total_cost()
    {
        _route.Sum(x => x.Cost).ShouldBe(35);
    }

    [Test]
    public void Solve_returns_expected_route()
    {
        // aaa -> bbb -> ddd -> ccc -> eee
        _route.Count.ShouldBe(4);

        // Note that direction of links is a bit random due to how they were initially defined
        // TODO     make link start+end evaluation more robust

        // aaa -> bbb
        _network.Nodes.Single(x => x.Id == _route[0].Start).Name.ShouldBe("aaa");
        _network.Nodes.Single(x => x.Id == _route[0].End).Name.ShouldBe("bbb");

        // bbb -> ddd
        _network.Nodes.Single(x => x.Id == _route[1].Start).Name.ShouldBe("ddd");
        _network.Nodes.Single(x => x.Id == _route[1].End).Name.ShouldBe("bbb");

        // ddd -> ccc
        _network.Nodes.Single(x => x.Id == _route[2].Start).Name.ShouldBe("ddd");
        _network.Nodes.Single(x => x.Id == _route[2].End).Name.ShouldBe("ccc");

        // ccc -> eee
        _network.Nodes.Single(x => x.Id == _route[3].Start).Name.ShouldBe("eee");
        _network.Nodes.Single(x => x.Id == _route[3].End).Name.ShouldBe("ccc");
    }
}
