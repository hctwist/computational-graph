using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors.Containers;

/// <summary>
/// A <see cref="DescriptorContainer{TDescriptor}"/> for node outputs.
/// </summary>
public class OutputDescriptorContainer : DescriptorContainer<IReadOnlySet<OutputDescriptor>>
{
    /// <inheritdoc />
    public OutputDescriptorContainer(Graph graph, NodeIdDesignator nodeIdDesignator) : base(graph, nodeIdDesignator)
    {
        graph.NodesFired += OnNodesFired;
    }

    /// <inheritdoc />
    public override IReadOnlySet<OutputDescriptor> Get()
    {
        return Graph.AllNodes.Select(node => new OutputDescriptor(NodeIdDesignator.DesignateId(node), node.DisplayOutput)).ToHashSet();
    }
    
    /// <summary>
    /// Called when nodes are fired.
    /// </summary>
    private void OnNodesFired()
    {
        NotifySubscribers(GetFired);
    }

    /// <summary>
    /// Gets output descriptors for all fired nodes.
    /// </summary>
    /// <returns>The descriptors.</returns>
    private IReadOnlySet<OutputDescriptor> GetFired()
    {
        HashSet<OutputDescriptor> firedOutputDescriptors = [];

        foreach (GraphNode node in Graph.AllNodes)
        {
            if (!node.WasFired)
            {
                continue;
            }

            firedOutputDescriptors.Add(new OutputDescriptor(NodeIdDesignator.DesignateId(node), node.DisplayOutput));
        }
        
        return firedOutputDescriptors;
    }
}