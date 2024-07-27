using ComputationalGraph.Core;
using ComputationalGraph.Visualisation.Descriptors;
using ComputationalGraph.Visualisation.Descriptors.Containers;

namespace ComputationalGraph.Visualisation;

/// <summary>
/// Facilitates getting an overview of graph structure/output and subscribing to any changes. 
/// </summary>
/// <remarks>
/// Note that nodes are assigned IDs for the purpose of this class,
/// which are only consistent within a single instance of <see cref="GraphVisualiser"/>.
/// </remarks>
public class GraphVisualiser
{
    /// <summary>
    /// Gets a descriptor container for nodes belonging to the graph.
    /// </summary>
    public DescriptorContainer<IReadOnlySet<NodeDescriptor>> Nodes { get; }
    
    /// <summary>
    /// Gets a descriptor container for edges on the graph.
    /// </summary>
    public DescriptorContainer<IReadOnlySet<EdgeDescriptor>> Edges { get; }
    
    /// <summary>
    /// Gets a descriptor container for node outputs.
    /// </summary>
    public DescriptorContainer<IReadOnlySet<OutputDescriptor>> Outputs { get; }
    
    /// <summary>
    /// Creates a new <see cref="GraphVisualiser"/>.
    /// </summary>
    /// <param name="graph">The graph.</param>
    public GraphVisualiser(Graph graph)
    {
        NodeIdDesignator nodeIdDesignator = new();

        Nodes = new NodeDescriptorContainer(graph, nodeIdDesignator);
        Edges = new EdgeDescriptorContainer(graph, nodeIdDesignator);
        Outputs = new OutputDescriptorContainer(graph, nodeIdDesignator);
    }
}