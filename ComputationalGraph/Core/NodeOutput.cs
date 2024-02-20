using ComputationalGraph.Exceptions;

namespace ComputationalGraph.Core;

/// <summary>
/// Output to a node's computation.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public readonly struct NodeOutput<TOutput>
{
    /// <summary>
    /// Gets whether the node has output.
    /// </summary>
    public bool HasOutput { get; }
    
    /// <summary>
    /// The output value if it exists.
    /// </summary>
    private readonly TOutput? value;

    /// <summary>
    /// Creates a new <see cref="NodeOutput{TOutput}"/>.
    /// </summary>
    /// <param name="hasOutput">Whether the node has output.</param>
    /// <param name="value">The output value if it exists.</param>
    private NodeOutput(bool hasOutput, TOutput? value)
    {
        HasOutput = hasOutput;
        this.value = value;
    }
    
    /// <summary>
    /// Gets the output value.
    /// </summary>
    /// <exception cref="NoOutputValueException">Thrown if there is no output.</exception>
    public TOutput Value => HasOutput ? value! : throw new NoOutputValueException($"Node has no output value. This should not be accessed outside of {nameof(Node<object>.Compute)}");

    /// <summary>
    /// Creates a node output with a value.
    /// </summary>
    /// <param name="value">The output value.</param>
    /// <returns>The output.</returns>
    public static NodeOutput<TOutput> From(TOutput value)
    {
        return new NodeOutput<TOutput>(true, value);
    }

    /// <summary>
    /// Creates a node output with no value.
    /// </summary>
    /// <returns>The output.</returns>
    public static NodeOutput<TOutput> Nothing()
    {
        return new NodeOutput<TOutput>(false, default);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return HasOutput ? Value!.ToString()! : "Nothing";
    }

    public static implicit operator NodeOutput<TOutput>(TOutput value)
    {
        return From(value);
    }
}