using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.BuildingBlocks;

public abstract class TransformNode<TInput, TOutput> : Node<TOutput>
{
    private readonly InputNode<TInput> inputNode;

    /// <inheritdoc />
    protected TransformNode(Graph graph, Node<TInput> inputNode) : base(graph)
    {
        this.inputNode = Input(inputNode);
    }

    /// <inheritdoc />
    public sealed override NodeOutput<TOutput> Compute()
    {
        return Compute(inputNode.Value);
    }

    protected abstract NodeOutput<TOutput> Compute(TInput input);
}