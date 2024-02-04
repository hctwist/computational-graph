using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Logic;

public class OrNode : AggregateNode<bool, bool>
{
    /// <inheritdoc />
    public OrNode(Graph graph, IEnumerable<Node<bool>> inputs) : base(graph, inputs)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<bool> Compute(IEnumerable<bool> inputs)
    {
        return inputs.Any(input => input);
    }
}