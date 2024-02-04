using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Logic;

public class AndNode : AggregateNode<bool, bool>
{
    /// <inheritdoc />
    public AndNode(Graph graph, IEnumerable<Node<bool>> inputs) : base(graph, inputs)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<bool> Compute(IEnumerable<bool> inputs)
    {
        return inputs.All(input => input);
    }
}