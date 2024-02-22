using System.Reflection;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Core;

public class FirePath
{
    public readonly IReadOnlyList<GraphNode> Path;

    public readonly IReadOnlyCollection<GraphNode> ConstantOutputPath;

    public FirePath(IReadOnlyList<GraphNode> path, IReadOnlyCollection<GraphNode> constantOutputPath)
    {
        Path = path;
        ConstantOutputPath = constantOutputPath;
    }

    public static FirePath Create(ISet<GraphNode> nodes)
    {
        Dictionary<GraphNode, int> depths = new(nodes.Count);

        Queue<GraphNode> nodesToResolve = new(nodes);

        while (nodesToResolve.TryDequeue(out GraphNode? node))
        {
            if (!TryGetInputDepth(node, depths, out int inputDepth))
            {
                nodesToResolve.Enqueue(node);
                continue;
            }

            depths.Add(node, inputDepth + 1);
        }

        IEnumerable<GraphNode> orderedNodes = depths
            .OrderBy(nodeWithDepth => nodeWithDepth.Value)
            .Select(nodeWithDepth => nodeWithDepth.Key);

        List<GraphNode> path = new(nodes.Count);
        List<GraphNode> constantOutputPath = new();
        
        foreach (GraphNode node in orderedNodes)
        {
            if (HasConstantOutput(node))
            {
                constantOutputPath.Add(node);
            }
            else
            {
                path.Add(node);
            }
        }

        return new FirePath(path, constantOutputPath);
    }

    private static bool HasConstantOutput(GraphNode node)
    {
        return node.GetType().GetCustomAttribute<ConstantOutputAttribute>() is not null;
    }

    private static bool TryGetInputDepth(GraphNode node, Dictionary<GraphNode, int> depths, out int inputDepth)
    {
        inputDepth = 0;

        foreach (GraphNode input in node.Inputs)
        {
            if (!depths.TryGetValue(input, out int depth))
            {
                return false;
            }

            inputDepth = Math.Max(inputDepth, depth);
        }

        return true;
    }
}