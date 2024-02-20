using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.BuildingBlocks;

public abstract class AggregateNode<TInput, TOutput> : Node<TOutput>
{
    private readonly IEnumerable<NodeInput<TInput>> inputs;

    /// <inheritdoc />
    protected AggregateNode(Graph graph, IEnumerable<Node<TInput>> inputs) : base(graph)
    {
        this.inputs = inputs.Select(Input).ToArray();
    }

    /// <inheritdoc />
    public sealed override NodeOutput<TOutput> Compute()
    {
        return Compute(inputs.Select(inputNode => inputNode.Value));
    }

    protected abstract NodeOutput<TOutput> Compute(IEnumerable<TInput> inputs);
}