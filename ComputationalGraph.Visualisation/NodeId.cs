namespace ComputationalGraph.Visualisation;

/// <summary>
/// An ID for a node. Note that this has no guarantees of uniqueness etc. from the graph itself.
/// </summary>
/// <param name="Id">The ID.</param>
public readonly record struct NodeId(int Id);