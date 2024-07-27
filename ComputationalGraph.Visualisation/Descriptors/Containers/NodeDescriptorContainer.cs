using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors.Containers;

/// <summary>
/// A <see cref="DescriptorContainer{TDescriptor}"/> for nodes.
/// </summary>
/// <remarks>This will publish all nodes on update.</remarks>
public class NodeDescriptorContainer : DescriptorContainer<IReadOnlySet<NodeDescriptor>>
{
    /// <inheritdoc />
    public NodeDescriptorContainer(Graph graph, NodeIdDesignator nodeIdDesignator) : base(graph, nodeIdDesignator)
    {
        graph.Primed += OnGraphPrimed;
    }

    /// <inheritdoc />
    public override IReadOnlySet<NodeDescriptor> Get()
    {
        return Graph.AllNodes
            .Select(node => new NodeDescriptor(NodeIdDesignator.DesignateId(node), node.Name))
            .ToHashSet();
    }

    /// <summary>
    /// Called when the graph is primed.
    /// </summary>
    private void OnGraphPrimed()
    {
        NotifySubscribers(Get);
    }
}