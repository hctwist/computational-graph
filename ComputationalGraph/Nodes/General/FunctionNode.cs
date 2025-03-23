
using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.General;

/// <summary>
/// A node which outputs a simple function of other nodes.
/// </summary>
/// <typeparam name="TInput1">The input type of node 1.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public class FunctionNode<TInput1, TOutput> : Node<TOutput>
{
    private readonly NodeInput<TInput1> input1;

    private readonly Func<TInput1, TOutput> function;

    /// <inheritdoc />
    public FunctionNode(
        Graph graph,
        Node<TInput1> input1,
        Func<TInput1, TOutput> function) : base(graph)
    {
        this.input1 = Input(input1);

        this.function = function;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return function(input1.Value);
    }
}

/// <summary>
/// A node which outputs a simple function of other nodes.
/// </summary>
/// <typeparam name="TInput1">The input type of node 1.</typeparam>
/// <typeparam name="TInput2">The input type of node 2.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public class FunctionNode<TInput1, TInput2, TOutput> : Node<TOutput>
{
    private readonly NodeInput<TInput1> input1;
    private readonly NodeInput<TInput2> input2;

    private readonly Func<TInput1, TInput2, TOutput> function;

    /// <inheritdoc />
    public FunctionNode(
        Graph graph,
        Node<TInput1> input1,
        Node<TInput2> input2,
        Func<TInput1, TInput2, TOutput> function) : base(graph)
    {
        this.input1 = Input(input1);
        this.input2 = Input(input2);

        this.function = function;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return function(input1.Value, input2.Value);
    }
}

/// <summary>
/// A node which outputs a simple function of other nodes.
/// </summary>
/// <typeparam name="TInput1">The input type of node 1.</typeparam>
/// <typeparam name="TInput2">The input type of node 2.</typeparam>
/// <typeparam name="TInput3">The input type of node 3.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public class FunctionNode<TInput1, TInput2, TInput3, TOutput> : Node<TOutput>
{
    private readonly NodeInput<TInput1> input1;
    private readonly NodeInput<TInput2> input2;
    private readonly NodeInput<TInput3> input3;

    private readonly Func<TInput1, TInput2, TInput3, TOutput> function;

    /// <inheritdoc />
    public FunctionNode(
        Graph graph,
        Node<TInput1> input1,
        Node<TInput2> input2,
        Node<TInput3> input3,
        Func<TInput1, TInput2, TInput3, TOutput> function) : base(graph)
    {
        this.input1 = Input(input1);
        this.input2 = Input(input2);
        this.input3 = Input(input3);

        this.function = function;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return function(input1.Value, input2.Value, input3.Value);
    }
}

/// <summary>
/// A node which outputs a simple function of other nodes.
/// </summary>
/// <typeparam name="TInput1">The input type of node 1.</typeparam>
/// <typeparam name="TInput2">The input type of node 2.</typeparam>
/// <typeparam name="TInput3">The input type of node 3.</typeparam>
/// <typeparam name="TInput4">The input type of node 4.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public class FunctionNode<TInput1, TInput2, TInput3, TInput4, TOutput> : Node<TOutput>
{
    private readonly NodeInput<TInput1> input1;
    private readonly NodeInput<TInput2> input2;
    private readonly NodeInput<TInput3> input3;
    private readonly NodeInput<TInput4> input4;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TOutput> function;

    /// <inheritdoc />
    public FunctionNode(
        Graph graph,
        Node<TInput1> input1,
        Node<TInput2> input2,
        Node<TInput3> input3,
        Node<TInput4> input4,
        Func<TInput1, TInput2, TInput3, TInput4, TOutput> function) : base(graph)
    {
        this.input1 = Input(input1);
        this.input2 = Input(input2);
        this.input3 = Input(input3);
        this.input4 = Input(input4);

        this.function = function;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return function(input1.Value, input2.Value, input3.Value, input4.Value);
    }
}

/// <summary>
/// A node which outputs a simple function of other nodes.
/// </summary>
/// <typeparam name="TInput1">The input type of node 1.</typeparam>
/// <typeparam name="TInput2">The input type of node 2.</typeparam>
/// <typeparam name="TInput3">The input type of node 3.</typeparam>
/// <typeparam name="TInput4">The input type of node 4.</typeparam>
/// <typeparam name="TInput5">The input type of node 5.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public class FunctionNode<TInput1, TInput2, TInput3, TInput4, TInput5, TOutput> : Node<TOutput>
{
    private readonly NodeInput<TInput1> input1;
    private readonly NodeInput<TInput2> input2;
    private readonly NodeInput<TInput3> input3;
    private readonly NodeInput<TInput4> input4;
    private readonly NodeInput<TInput5> input5;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TOutput> function;

    /// <inheritdoc />
    public FunctionNode(
        Graph graph,
        Node<TInput1> input1,
        Node<TInput2> input2,
        Node<TInput3> input3,
        Node<TInput4> input4,
        Node<TInput5> input5,
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TOutput> function) : base(graph)
    {
        this.input1 = Input(input1);
        this.input2 = Input(input2);
        this.input3 = Input(input3);
        this.input4 = Input(input4);
        this.input5 = Input(input5);

        this.function = function;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        return function(input1.Value, input2.Value, input3.Value, input4.Value, input5.Value);
    }
}
