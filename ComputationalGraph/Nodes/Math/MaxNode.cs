using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Math;

public class MaxNode<TInput> : AggregateNode<TInput, TInput>
{
    /// <inheritdoc />
    public MaxNode(Graph graph, IEnumerable<Node<TInput>> inputs) : base(graph, inputs)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<TInput> Compute(IEnumerable<TInput> inputs)
    {
        return inputs.Any() ? inputs.Max(input => input)! : Nothing();
    }
}