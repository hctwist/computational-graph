using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.General;

/// <summary>
/// A node with a constant value.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public class ConstantNode<TOutput> : Node<TOutput>
{
    /// <summary>
    /// The constant output.
    /// </summary>
    private readonly TOutput output;

    /// <inheritdoc />
    public ConstantNode(Graph graph, TOutput output) : base(graph)
    {
        this.output = output;
    }

    /// <inheritdoc />
    public override NodeOutput<TOutput> Compute()
    {
        return output;
    }
}