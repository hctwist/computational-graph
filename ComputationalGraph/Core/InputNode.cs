namespace ComputationalGraph.Core;

/// <summary>
/// A node that can provide input to <see cref="Node{TOutput}.Compute"/>.
/// </summary>
/// <typeparam name="TOutput">The node output type.</typeparam>
public class InputNode<TOutput>
{
    /// <summary>
    /// The output node.
    /// </summary>
    private readonly Node<TOutput> backingNode;

    /// <summary>
    /// Creates a new <see cref="InputNode{TOutput}"/>.
    /// </summary>
    /// <param name="backingNode">The output node.</param>
    internal InputNode(Node<TOutput> backingNode)
    {
        this.backingNode = backingNode;
    }

    /// <summary>
    /// Gets the current input value.
    /// </summary>
    /// <remarks>This is only safe to call in <see cref="Node{TOutput}.Compute"/>.</remarks>
    public TOutput Value => backingNode.LastOutput.Value;
}