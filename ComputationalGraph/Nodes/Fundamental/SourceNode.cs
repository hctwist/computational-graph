using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.Fundamental;

/// <summary>
/// A node that can be fired with a value.
/// This is the only type of node that can trigger a graph fire path.
/// </summary>
/// <typeparam name="TOutput">The node output.</typeparam>
public sealed class SourceNode<TOutput> : Node<TOutput>
{
    /// <summary>
    /// The current output.
    /// </summary>
    private NodeOutput<TOutput> output;

    /// <inheritdoc />
    public SourceNode(Graph graph) : this(graph, Nothing())
    {
    }

    /// <summary>
    /// Creates a new <see cref="SourceNode{TOutput}"/>.
    /// </summary>
    /// <param name="graph">The graph.</param>
    /// <param name="initialOutput">The initial output.</param>
    public SourceNode(Graph graph, NodeOutput<TOutput> initialOutput) : base(graph)
    {
        output = initialOutput;
    }

    /// <summary>
    /// Fires the node.
    /// This will trigger a graph fire path.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Fire(NodeOutput<TOutput> value)
    {
        output = value;
        Graph.Fire(this);
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return output;
    }
}