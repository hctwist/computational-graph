namespace ComputationalGraph.Core;

/// <summary>
/// A graph node.
/// </summary>
public abstract class GraphNode
{
    /// <summary>
    /// Gets the name of the node.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets whether the node was fired in the latest graph fire.
    /// </summary>
    public abstract bool WasFired { get; }
    
    /// <summary>
    /// Gets or sets the node's index in the graph's fire path, or null if it isn't in the path.
    /// </summary>
    internal int? PathIndex { get; set; }
    
    /// <summary>
    /// Gets the node's inputs.
    /// </summary>
    internal abstract IReadOnlySet<GraphNode> Inputs { get; }

    /// <summary>
    /// Gets whether the node had output last time it was fired.
    /// </summary>
    internal abstract bool LastHadOutput { get; }

    /// <summary>
    /// Gets the node's last display output.
    /// </summary>
    internal abstract string LastDisplayOutput { get; }

    /// <summary>
    /// Determines whether this node should fire.
    /// </summary>
    internal abstract bool ShouldFire();
    
    /// <summary>
    /// Fires this node.
    /// </summary>
    internal abstract void Fire();

    /// <summary>
    /// Creates a new <see cref="GraphNode"/>.
    /// </summary>
    protected GraphNode()
    {
        Name = GetType().Name;
    }
}