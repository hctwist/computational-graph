using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Math;

public class MinNode<TInput> : AggregateNode<TInput, TInput>
{
    /// <inheritdoc />
    public MinNode(Graph graph, IEnumerable<Node<TInput>> inputs) : base(graph, inputs)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<TInput> Compute(IEnumerable<TInput> inputs)
    {
        return inputs.Any() ? inputs.Min(input => input)! : Nothing();
    }
}