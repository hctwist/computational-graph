using ComputationalGraph.Core;

namespace ComputationalGraph.Observing;

public delegate void OnNodeFired(GraphNode node, NodeOutput<string?> output);