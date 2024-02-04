using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Tests.Nodes;

public class PassthroughNode<TOutput> : TransformNode<TOutput, TOutput>
{
    /// <inheritdoc />
    public PassthroughNode(Graph graph, Node<TOutput> inputNode) : base(graph, inputNode)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute(TOutput input)
    {
        return input;
    }
}