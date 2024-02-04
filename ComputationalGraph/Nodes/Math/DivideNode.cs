﻿using System.Numerics;
using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.Math;

public class DivideNode<TNumber> : Node<TNumber> where TNumber : INumber<TNumber>
{
    private readonly InputNode<TNumber> left;
    private readonly InputNode<TNumber> right;
    
    /// <inheritdoc />
    public DivideNode(Graph graph, Node<TNumber> left, Node<TNumber> right) : base(graph)
    {
        this.left = Input(left);
        this.right = Input(right);
    }

    /// <inheritdoc />
    public override NodeOutput<TNumber> Compute()
    {
        return left.Value / right.Value;
    }
}