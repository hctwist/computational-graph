using ComputationalGraph.Exceptions;

namespace ComputationalGraph.Core;

/// <summary>
/// A node that can provide input to <see cref="Node{TOutput}.Compute"/>.
/// </summary>
/// <typeparam name="TOutput">The node output type.</typeparam>
public class NodeInput<TOutput>
{
    /// <summary>
    /// The node providing input.
    /// </summary>
#pragma warning disable CGRAPH0001
    private readonly Node<TOutput> backingNode;
#pragma warning restore CGRAPH0001

    /// <summary>
    /// Creates a new <see cref="NodeInput{TOutput}"/>.
    /// </summary>
    /// <param name="backingNode">The input node.</param>
    internal NodeInput(Node<TOutput> backingNode)
    {
        this.backingNode = backingNode;
    }

    /// <summary>
    /// Gets the current input value.
    /// </summary>
    /// <remarks>This is only safe to call in <see cref="Node{TOutput}.Compute"/>.</remarks>
    /// <exception cref="InvalidGraphStateException">Thrown if this is called whilst the graph is not firing.</exception>
    public TOutput Value
    {
        get
        {
            if (backingNode.Graph.State is not (GraphState.Priming or GraphState.Firing))
            {
                throw new InvalidGraphStateException($"Cannot access an input node's value whilst the graph is in the state {backingNode.Graph.State}");
            }
            
            return backingNode.LastOutput.Value;
        }
    }
}