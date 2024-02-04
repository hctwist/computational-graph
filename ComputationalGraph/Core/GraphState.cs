namespace ComputationalGraph.Core;

/// <summary>
/// States of the graph.
/// </summary>
public enum GraphState
{
    /// <summary>
    /// Before the graph has been primed. This is the only state in which nodes can be added.
    /// </summary>
    Building,
    
    /// <summary>
    /// The graph is priming.
    /// </summary>
    Priming,
    
    /// <summary>
    /// The graph is ready for nodes to be fired.
    /// </summary>
    Idle,
    
    /// <summary>
    /// The graph is firing.
    /// </summary>
    Firing,
    
    /// <summary>
    /// The graph is accepting a batch.
    /// </summary>
    /// <remarks>This is just the batch collection state, when the batch is firing the graph will be in state <see cref="Firing"/>.</remarks>
    Batching
}