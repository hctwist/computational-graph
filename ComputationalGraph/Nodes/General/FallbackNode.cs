using ComputationalGraph.Core;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Nodes.General;

/// <summary>
/// A node which falls back to another node's output if the input node doesn't have an output.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public sealed class FallbackNode<TOutput> : Node<TOutput>
{
    /// <summary>
    /// The input node.
    /// </summary>
    private readonly NodeInput<TOutput> input;

    /// <inheritdoc />
    public FallbackNode(Graph graph, Node<TOutput> input, Node<TOutput> fallback) : base(graph, fallback)
    {
        this.input = Input(input);
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return input.Value;
    }
}

public static class FallbackNodeExtensions
{
    /// <summary>
    /// Adds a fallback in front of this node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="fallback">The fallback node.</param>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <returns>The new node.</returns>
    public static Node<TOutput> WithFallback<TOutput>(this Node<TOutput> node, Node<TOutput> fallback)
    {
        return new FallbackNode<TOutput>(node.Graph, node, fallback);
    }

    /// <summary>
    /// Adds a fallback in front of this node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="fallback">The fallback value.</param>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <returns>The new node.</returns>
    public static Node<TOutput> WithFallback<TOutput>(this Node<TOutput> node, TOutput fallback)
    {
        return new FallbackNode<TOutput>(node.Graph, node, new ConstantNode<TOutput>(node.Graph, fallback));
    }
}