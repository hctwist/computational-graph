using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation;

/// <summary>
/// Assigns IDs to nodes.
/// </summary>
public class NodeIdDesignator
{
    /// <summary>
    /// Currently assigned node IDs.
    /// </summary>
    private readonly Dictionary<GraphNode, NodeId> nodeIds;

    /// <summary>
    /// The next node ID to use.
    /// </summary>
    private int nextNodeId;

    /// <summary>
    /// Creates a new <see cref="NodeIdDesignator"/>.
    /// </summary>
    public NodeIdDesignator()
    {
        nodeIds = new Dictionary<GraphNode, NodeId>();
        nextNodeId = 0;
    }
    
    /// <summary>
    /// Gets the ID for a node, or creates (and designates) one if the node has no ID.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The node ID.</returns>
    public NodeId DesignateId(GraphNode node)
    {
        if (!nodeIds.TryGetValue(node, out NodeId id))
        {
            id = new NodeId(nextNodeId++);
            nodeIds.Add(node, id);
        }

        return id;
    }
}