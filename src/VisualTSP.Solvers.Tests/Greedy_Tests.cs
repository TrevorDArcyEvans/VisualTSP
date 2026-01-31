namespace VisualTSP.Solvers.Tests;

using Models;
using Newtonsoft.Json;
using Serialisation;
using Shouldly;

[TestFixture]
public sealed class Greedy_Tests
{
    private static (Network, List<Link>) Solve(string networkFileName)
    {
        var filePath = Path.Combine("samples", networkFileName);
        var json = File.ReadAllText(filePath);
        var jsonNetwork = JsonConvert.DeserializeObject<JsonNetwork>(json);
        var network = jsonNetwork!.ToNetwork();
        var solver = new Greedy(network);
        var route = solver.Solve();

        return (network, route);
    }

    [Test]
    public void Solve_ref_returns_expected_total_cost()
    {
        var (_, route) = Solve("reference.tsp");

        route.Sum(x => x.Cost).ShouldBe(35);
    }

    [Test]
    public void Solve_ref_returns_expected_route()
    {
        var (network, route) = Solve("reference.tsp");

        // aaa -> bbb -> ddd -> ccc -> eee
        route.Count.ShouldBe(4);

        // Note that direction of links is a bit random due to how they were initially defined
        // TODO     make link start+end evaluation more robust

        // aaa -> bbb
        network.Nodes.Single(x => x.Id == route[0].Start).Name.ShouldBe("aaa");
        network.Nodes.Single(x => x.Id == route[0].End).Name.ShouldBe("bbb");

        // bbb -> ddd
        network.Nodes.Single(x => x.Id == route[1].Start).Name.ShouldBe("ddd");
        network.Nodes.Single(x => x.Id == route[1].End).Name.ShouldBe("bbb");

        // ddd -> ccc
        network.Nodes.Single(x => x.Id == route[2].Start).Name.ShouldBe("ddd");
        network.Nodes.Single(x => x.Id == route[2].End).Name.ShouldBe("ccc");

        // ccc -> eee
        network.Nodes.Single(x => x.Id == route[3].Start).Name.ShouldBe("eee");
        network.Nodes.Single(x => x.Id == route[3].End).Name.ShouldBe("ccc");
    }

    [Test]
    public void Solve_hub_spoke_returns_empty()
    {
        var (_, route) = Solve("hub-spoke.tsp");
        
        route.ShouldBe(Array.Empty<Link>());
    }
}
