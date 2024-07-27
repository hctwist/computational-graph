using System.Reflection;
using ComputationalGraph.Exceptions;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Core;

/// <summary>
/// Graph fire path.
/// </summary>
internal class FirePath
{
    /// <summary>
    /// The (non-constant) path.
    /// </summary>
    private readonly List<GraphNode> path;

    /// <summary>
    /// Nodes with constant output.
    /// </summary>
    private readonly List<GraphNode> constantOutputNodes;

    /// <summary>
    /// Creates a new <see cref="FirePath"/>.
    /// </summary>
    public FirePath()
    {
        path = new List<GraphNode>();
        constantOutputNodes = new List<GraphNode>();
    }

    /// <summary>
    /// Gets the current fire path.
    /// </summary>
    public IReadOnlyList<GraphNode> Path => path;

    /// <summary>
    /// Gets the nodes with constant output.
    /// </summary>
    /// <remarks>These are excluded from <see cref="Path"/>.</remarks>
    public IReadOnlyCollection<GraphNode> ConstantOutputNodes => constantOutputNodes;

    /// <summary>
    /// Adds a node to the path.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <remarks>This will be added to the end of the path before calling <see cref="Resolve"/>.</remarks>
    public void Add(GraphNode node)
    {
        if (HasConstantOutput(node))
        {
            constantOutputNodes.Add(node);
        }
        else
        {
            path.Add(node);
        }
    }

    /// <summary>
    /// Sorts the fire path so that all dependencies are resolved in the correct order.
    /// </summary>
    /// <exception cref="GraphCycleException">Thrown if there is a cycle in the path.</exception>
    public void Resolve()
    {
        // Constant output nodes are implicitly resolved
        HashSet<GraphNode> resolved = new(constantOutputNodes);
        
        // The stack of nodes to resolve
        Stack<GraphNode> nodesToResolve = new(Enumerable.Reverse(path));

        // Keep track of all encountered input nodes, that haven't yet been resolved
        HashSet<GraphNode> unresolvedInputs = new();
        
        // Clear the path and repopulate with the resolved nodes
        path.Clear();
        
        while (nodesToResolve.TryPeek(out GraphNode? nodeToResolve))
        {
            // Skip the node if already resolved
            if (resolved.Contains(nodeToResolve))
            {
                nodesToResolve.Pop();
                continue;
            }

            bool allInputsResolved = true;

            foreach (GraphNode input in nodeToResolve.Inputs)
            {
                // If the input has been encountered already and was unresolved, there must be a cycle
                if (unresolvedInputs.Contains(input))
                {
                    throw new GraphCycleException($"Cycle detected with node {input.Name}. This is not supported by the fire path");
                }
                
                // If the input hasn't been resolved, push it onto the stack to resolve
                if (!resolved.Contains(input))
                {
                    allInputsResolved = false;
                    nodesToResolve.Push(input);
                    unresolvedInputs.Add(input);
                }
            }

            if (allInputsResolved)
            {
                // Add the node to the final path if resolved
                path.Add(nodeToResolve);
                
                resolved.Add(nodeToResolve);
                nodesToResolve.Pop();
                unresolvedInputs.Remove(nodeToResolve);
            }
        }
    }

    private static bool HasConstantOutput(GraphNode node)
    {
        return node.GetType().GetCustomAttribute<ConstantOutputAttribute>() is not null;
    }
}