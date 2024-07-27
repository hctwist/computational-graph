using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors.Containers;

/// <summary>
/// A <see cref="DescriptorContainer{TDescriptor}"/> for edges between nodes.
/// </summary>
/// <remarks>This will publish all edges on update.</remarks>
public class EdgeDescriptorContainer : DescriptorContainer<IReadOnlySet<EdgeDescriptor>>
{
    /// <inheritdoc />
    public EdgeDescriptorContainer(Graph graph, NodeIdDesignator nodeIdDesignator) : base(graph, nodeIdDesignator)
    {
        graph.Primed += OnGraphPrimed;
    }

    /// <inheritdoc />
    public override IReadOnlySet<EdgeDescriptor> Get()
    {
        HashSet<EdgeDescriptor> descriptors = [];

        foreach (GraphNode node in Graph.AllNodes)
        {
            NodeId nodeId = NodeIdDesignator.DesignateId(node);

            foreach (GraphNode input in node.Inputs)
            {
                descriptors.Add(new EdgeDescriptor(NodeIdDesignator.DesignateId(input), nodeId));
            }
        }

        return descriptors;
    }

    /// <summary>
    /// Called when the graph is primed.
    /// </summary>
    private void OnGraphPrimed()
    {
        NotifySubscribers(Get);
    }
}