namespace VisualTSP.Solvers;

using Models;

public class Greedy(Network network)
{
    private readonly Network _network = network;

    public List<Link> Solve()
    {
        if (_network.Nodes.Count == 0)
        {
            return new List<Link>();
        }

        var route = new List<Link>();
        var remainingNodes = new HashSet<Node>(_network.Nodes);
        var startNode = _network.Nodes.Single(x => x.Id == _network.Start);
        var endNode = _network.Nodes.Single(x => x.Id == _network.End);
        var lastNode = startNode;
        remainingNodes.Remove(lastNode);
        while (remainingNodes.Count > 0)
        {
            var minCost = int.MaxValue;
            Link minLink = default;
            Node closestNode = default;

            var otherNodes = remainingNodes.Count > 1 ? remainingNodes.Except(endNode) : remainingNodes;

            foreach (var node in otherNodes)
            {
                // TODO     support asymmetric links
                // look for a link to/from last node to this node
                var link = _network.Links
                    .FirstOrDefault(x =>
                        (x.Start == lastNode.Id && x.End == node.Id) ||
                        (x.End == lastNode.Id && x.Start == node.Id));
                if (link is null)
                {
                    continue;
                }

                if (link.Cost < minCost)
                {
                    minCost = link.Cost;
                    minLink = link;
                    closestNode = node;
                }
            }

            remainingNodes.Remove(closestNode);
            route.Add(minLink);
            lastNode = closestNode;
        }

        return route;
    }
}
