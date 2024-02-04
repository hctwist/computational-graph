using ComputationalGraph.Core;
using ComputationalGraph.Nodes.BuildingBlocks;

namespace ComputationalGraph.Nodes.Logic;

public class NotNode : TransformNode<bool, bool>
{
    /// <inheritdoc />
    public NotNode(Graph graph, Node<bool> inputNode) : base(graph, inputNode)
    {
    }

    /// <inheritdoc />
    protected override NodeOutput<bool> Compute(bool input)
    {
        return !input;
    }
}