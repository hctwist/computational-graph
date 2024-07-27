using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.Fundamental;

/// <summary>
/// A node with a constant value.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
[ConstantOutput]
public sealed class ConstantNode<TOutput> : Node<TOutput>
{
    /// <summary>
    /// The constant output.
    /// </summary>
    private readonly NodeOutput<TOutput> output;

    /// <inheritdoc />
    public ConstantNode(Graph graph, NodeOutput<TOutput> output) : base(graph)
    {
        Name = $"Constant ({output})";
        this.output = output;
    }

    /// <inheritdoc />
    public override NodeOutput<TOutput> Compute()
    {
        return output;
    }
}