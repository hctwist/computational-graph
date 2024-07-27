using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors;

/// <summary>
/// Descriptor for an edge between a source and target <see cref="GraphNode"/>.
/// </summary>
/// <param name="Source">The source node (an input to the target node).</param>
/// <param name="Target">The target node.</param>
public record EdgeDescriptor(NodeId Source, NodeId Target);