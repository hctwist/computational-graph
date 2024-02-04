using System.Numerics;
using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Math;

public class AddNode<TNumber> : AggregateNode<TNumber, TNumber> where TNumber : INumber<TNumber>
{
    /// <inheritdoc />
    public AddNode(Graph graph, params Node<TNumber>[] operands) : base(graph, operands)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<TNumber> Compute(IEnumerable<TNumber> inputs)
    {
        return inputs.Aggregate(TNumber.Zero, (result, input) => result + input);
    }
}