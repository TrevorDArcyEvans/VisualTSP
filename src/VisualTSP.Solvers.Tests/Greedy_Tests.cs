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

        _network.Nodes.Single(x => x.Id == _route[0].Start).Name.ShouldBe("aaa");
    }
}
