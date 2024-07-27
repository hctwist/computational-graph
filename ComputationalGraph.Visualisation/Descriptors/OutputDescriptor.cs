using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors;

/// <summary>
/// Descriptor for the output of a <see cref="Node{TOutput}"/>.
/// </summary>
/// <param name="Id">The node ID.</param>
/// <param name="Output">The node's display output.</param>
public record OutputDescriptor(NodeId Id, NodeOutput<string?> Output);