using ComputationalGraph.Core;

namespace ComputationalGraph.Edges;

/// <summary>
/// Helps to visualise a graph and its nodes.
/// </summary>
/// <remarks>
/// Note that nodes are assigned IDs for the purpose of this class,
/// which are only consistent within a single instance of <see cref="GraphVisualiser"/>.
/// </remarks>
public class GraphVisualiser
{
    /// <summary>
    /// Occurs when output descriptors are updated, ie. nodes are fired.
    /// </summary>
    public event Action<IReadOnlySet<OutputDescriptor>>? OutputDescriptorsUpdated;

    /// <summary>
    /// The graph.
    /// </summary>
    private readonly Graph graph;

    /// <summary>
    /// Currently assigned node IDs.
    /// </summary>
    private readonly Dictionary<GraphNode, NodeId> nodeIds;

    /// <summary>
    /// The next node ID to use.
    /// </summary>
    private int nextNodeId;

    /// <summary>
    /// Creates a new <see cref="GraphVisualiser"/>.
    /// </summary>
    /// <param name="graph">The graph.</param>
    public GraphVisualiser(Graph graph)
    {
        this.graph = graph;
        graph.NodesFired += OnNodesFired;

        nodeIds = new Dictionary<GraphNode, NodeId>();
        nextNodeId = 0;
    }

    /// <summary>
    /// Builds descriptors for all nodes in the graph.
    /// </summary>
    /// <returns>The descriptor.</returns>
    public IReadOnlySet<NodeDescriptor> BuildNodeDescriptors()
    {
        return graph.AllNodes
            .Select(node => new NodeDescriptor(GetOrAssignNodeId(node), node.Name))
            .ToHashSet();
    }

    /// <summary>
    /// Builds descriptors for all edges in the graph.
    /// </summary>
    /// <returns>The descriptor.</returns>
    public IReadOnlySet<EdgeDescriptor> BuildEdgeDescriptors()
    {
        HashSet<EdgeDescriptor> descriptors = [];

        foreach (GraphNode node in graph.AllNodes)
        {
            NodeId nodeId = GetOrAssignNodeId(node);

            foreach (GraphNode input in node.Inputs)
            {
                descriptors.Add(new EdgeDescriptor(GetOrAssignNodeId(input), nodeId));
            }
        }

        return descriptors;
    }

    /// <summary>
    /// Builds descriptors for all node outputs.
    /// </summary>
    /// <returns>The descriptor.</returns>
    public IReadOnlySet<OutputDescriptor> BuildOutputDescriptors()
    {
        return graph.AllNodes.Select(node => new OutputDescriptor(GetOrAssignNodeId(node), node.DisplayOutput)).ToHashSet();
    }

    /// <summary>
    /// Called when nodes are fired.
    /// </summary>
    private void OnNodesFired()
    {
        if (OutputDescriptorsUpdated is null)
        {
            return;
        }

        HashSet<OutputDescriptor> updatedOutputDescriptors = [];

        foreach (GraphNode node in graph.AllNodes)
        {
            if (!node.WasFired)
            {
                continue;
            }

            updatedOutputDescriptors.Add(new OutputDescriptor(GetOrAssignNodeId(node), node.DisplayOutput));
        }

        OutputDescriptorsUpdated?.Invoke(updatedOutputDescriptors);
    }

    /// <summary>
    /// Gets the ID for a node, or creates (and assigns) one if the node has no ID.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The node ID.</returns>
    private NodeId GetOrAssignNodeId(GraphNode node)
    {
        if (!nodeIds.TryGetValue(node, out NodeId id))
        {
            id = new NodeId(nextNodeId++);
            nodeIds.Add(node, id);
        }

        return id;
    }
}