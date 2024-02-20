using System.Numerics;
using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.Math;

public class SubtractNode<TNumber> : Node<TNumber> where TNumber : INumber<TNumber>
{
    private readonly NodeInput<TNumber> left;
    private readonly NodeInput<TNumber> right;
    
    /// <inheritdoc />
    public SubtractNode(Graph graph, Node<TNumber> left, Node<TNumber> right) : base(graph)
    {
        this.left = Input(left);
        this.right = Input(right);
    }

    /// <inheritdoc />
    public override NodeOutput<TNumber> Compute()
    {
        return left.Value - right.Value;
    }
}