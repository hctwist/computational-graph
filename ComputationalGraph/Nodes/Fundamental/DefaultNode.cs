using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;
using ComputationalGraph.Nodes.General;

namespace ComputationalGraph.Nodes.Fundamental;

/// <summary>
/// A node which falls back to a default value if the input node doesn't have an output.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public sealed class DefaultNode<TOutput> : TransformNode<TOutput, TOutput>
{
    /// <summary>
    /// The default node.
    /// </summary>
    private readonly InputNode<TOutput> @default;

    /// <inheritdoc />
    public DefaultNode(Graph graph, Node<TOutput> input, Node<TOutput> @default) : base(graph, input)
    {
        this.@default = Input(@default);
    }

    /// <inheritdoc />
    public DefaultNode(Graph graph, Node<TOutput> input, TOutput @default) : base(graph, input)
    {
        this.@default = Input(new ConstantNode<TOutput>(graph, @default));
    }

    /// <inheritdoc />
    private protected override NodeOutput<TOutput> ComputeFallback()
    {
        return @default.Value;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute(TOutput input)
    {
        return input;
    }
}