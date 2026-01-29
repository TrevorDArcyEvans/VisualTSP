namespace VisualTSP.Solvers.Tests;

using Newtonsoft.Json;
using Serialisation;
using Shouldly;

[TestFixture]
public class Greedy_Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Solve_returns_expected_total_cost()
    {
        var filePath = Path.Combine("samples", "New Document.tsp");
        var json = File.ReadAllText(filePath);
        var jsonNetwork = JsonConvert.DeserializeObject<JsonNetwork>(json);
        var network = jsonNetwork.ToNetwork();
        var solver = new Greedy(network);
        
        var route = solver.Solve();

        route.Sum(x => x.Cost).ShouldBe(35);
    }
}
