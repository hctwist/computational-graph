using ComputationalGraph.Exceptions;

namespace ComputationalGraph.Core;

/// <summary>
/// A graph node.
/// </summary>
public abstract class GraphNode
{
    /// <summary>
    /// The graph this node belongs to.
    /// </summary>
    public Graph Graph { get; }

    /// <summary>
    /// Gets the name of the node.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets or sets whether this node has been primed.
    /// </summary>
    internal bool Primed { get; set; }

    /// <summary>
    /// Gets or sets the node's index in the graph's fire path, or null if it isn't in the path.
    /// </summary>
    internal int? PathIndex { get; set; }

    /// <summary>
    /// Gets whether the node had output last time it was fired.
    /// </summary>
    internal abstract bool LastHadOutput { get; }

    /// <summary>
    /// Gets the node's last output value.
    /// </summary>
    internal abstract object? LastOutputValue { get; }
    
    /// <summary>
    /// The node's inputs.
    /// </summary>
    /// <remarks>This is exposed internally to keep enumerating allocation free.</remarks>
    private protected readonly HashSet<GraphNode> InputsInternal;

    /// <summary>
    /// The node's version, or null if the node hasn't been fired.
    /// </summary>
    private protected int? Version;

    /// <summary>
    /// Creates a new <see cref="GraphNode"/>.
    /// </summary>
    protected GraphNode(Graph graph)
    {
        Graph = graph;
        graph.AddNode(this);

        Name = GetType().Name;

        Primed = false;
        PathIndex = null;

        Version = null;
        InputsInternal = [];
    }

    /// <summary>
    /// Gets the node's output.
    /// </summary>
    public NodeOutput<object?> Output
    {
        get
        {
            EnsureOutputCanBeAccessed();
            return LastHadOutput ? NodeOutput<object?>.Nothing() : LastOutputValue;
        }
    }

    /// <summary>
    /// Gets whether the node was fired in the latest graph fire.
    /// </summary>
    public bool WasFired => Version == Graph.Version;

    /// <summary>
    /// Gets the node's inputs.
    /// </summary>
    public IReadOnlySet<GraphNode> Inputs => InputsInternal;

    /// <summary>
    /// Determines whether this node should fire.
    /// </summary>
    internal bool ShouldFire()
    {
        foreach (GraphNode inputNode in InputsInternal)
        {
            if (inputNode.WasFired)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Fires this node.
    /// </summary>
    internal abstract void Fire();

    /// <summary>
    /// Ensures that the graph is in a valid state for output to be accessed.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    protected void EnsureOutputCanBeAccessed()
    {
        if (Graph.State is not GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Attempted to read output whilst the graph was {Graph.State}");
        }
    }
}