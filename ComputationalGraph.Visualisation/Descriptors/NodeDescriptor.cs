namespace ComputationalGraph.Visualisation.Descriptors;

/// <summary>
/// Descriptor for a <see cref="ComputationalGraph.Core.GraphNode"/>.
/// </summary>
/// <param name="Id">The node ID.</param>
/// <param name="Name">The node name.</param>
public record NodeDescriptor(NodeId Id, string Name);