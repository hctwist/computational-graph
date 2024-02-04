using ComputationalGraph.Exceptions;

namespace ComputationalGraph.Core;

/// <summary>
/// A graph node.
/// </summary>
public abstract class Node
{
    /// <summary>
    /// Gets the name of the node.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets whether the node had output last time it was fired.
    /// </summary>
    internal abstract bool LastHadOutput { get; }

    /// <summary>
    /// Gets the node's last display output.
    /// </summary>
    internal abstract string LastDisplayOutput { get; }

    /// <summary>
    /// Gets the input nodes to this node.
    /// </summary>
    internal abstract IEnumerable<Node> Inputs { get; }

    /// <summary>
    /// Gets all nodes (directly) dependent on this node.
    /// </summary>
    internal abstract IEnumerable<Node> Dependents { get; }

    /// <summary>
    /// Fires this node.
    /// </summary>
    internal abstract void Fire();
}

/// <summary>
/// A graph node.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public abstract class Node<TOutput> : Node
{
    /// <summary>
    /// Occurs when the node is fired.
    /// </summary>
    /// <remarks>This is called immediately as the node is fired.</remarks>
    public event Action<NodeOutput<TOutput>>? Fired;

    /// <summary>
    /// The last output of this node.
    /// </summary>
    internal NodeOutput<TOutput> LastOutput;

    /// <inheritdoc />
    internal override HashSet<Node> Inputs { get; }

    /// <inheritdoc />
    internal override HashSet<Node> Dependents { get; }

    /// <summary>
    /// The graph this node belongs to.
    /// </summary>
    private protected readonly Graph Graph;

    /// <summary>
    /// The node's ID.
    /// </summary>
    private readonly int id;

    /// <summary>
    /// The node's version, or null if the node hasn't been fired.
    /// </summary>
    private int? version;

    /// <summary>
    /// Creates a new <see cref="Node"/>.
    /// </summary>
    /// <param name="graph">The graph to add this node to.</param>
    protected Node(Graph graph)
    {
        LastOutput = Nothing();

        Graph = graph;
        id = graph.AddNode(this);

        Inputs = new HashSet<Node>();
        Dependents = new HashSet<Node>();

        version = null;
    }

    /// <summary>
    /// Gets the node's output.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    public NodeOutput<TOutput> Output
    {
        get
        {
            if (Graph.State is GraphState.Firing or GraphState.Priming)
            {
                throw new InvalidGraphStateException($"Attempted to read output whilst the graph was {Graph.State}");
            }

            return LastOutput;
        }
    }

    /// <summary>
    /// Gets whether the node was fired in the latest graph fire.
    /// </summary>
    public bool WasFired => version == Graph.Version;

    /// <inheritdoc />
    public override string Name => $"{GetType().Name} ~ {id}";

    /// <inheritdoc />
    internal sealed override bool LastHadOutput => LastOutput.HasOutput;

    /// <inheritdoc />
    internal sealed override string LastDisplayOutput => LastOutput.Value?.ToString() ?? string.Empty;

    /// <inheritdoc />
    internal sealed override void Fire()
    {
        LastOutput = Inputs.Any(input => !input.LastHadOutput) ? ComputeFallback() : Compute();
        version = Graph.Version;
        Fired?.Invoke(LastOutput);
    }

    /// <summary>
    /// Gets the fallback node output for when some of the node's inputs have no output.
    /// </summary>
    /// <returns>The fallback output.</returns>
    private protected virtual NodeOutput<TOutput> ComputeFallback()
    {
        return Nothing();
    }

    /// <summary>
    /// Registers an input node.
    /// This should be the single route in which nodes can take output from other nodes.
    /// </summary>
    /// <param name="inputNode">The input node.</param>
    /// <typeparam name="TInput">The output type of the input node.</typeparam>
    /// <returns>The input node.</returns>
    /// <exception cref="InvalidNodeInputException">Thrown if this node cannot be an input for this node.</exception>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    protected InputNode<TInput> Input<TInput>(Node<TInput> inputNode)
    {
        if (inputNode.Graph != Graph)
        {
            throw new InvalidNodeInputException($"Tried to add node {inputNode.Name} from a different graph as an input to node {Name}");
        }

        if (Graph.State != GraphState.Building)
        {
            throw new InvalidGraphStateException($"Tried to add node {inputNode.Name} as an input to node {Name} whilst the graph is {Graph.State}");
        }

        if (!Inputs.Add(inputNode))
        {
            throw new InvalidNodeInputException($"Node {inputNode} has already been added as an input of {this}");
        }

        if (!inputNode.Dependents.Add(this))
        {
            throw new InvalidNodeInputException($"Node {inputNode} has already been added as a dependent of {this}");
        }

        return new InputNode<TInput>(inputNode);
    }

    /// <summary>
    /// Gets a node output representing no output.
    /// </summary>
    /// <returns>The node output.</returns>
    protected static NodeOutput<TOutput> Nothing()
    {
        return NodeOutput<TOutput>.Nothing();
    }

    /// <summary>
    /// Computes this node's output.
    /// </summary>
    /// <remarks>During this call it is safe to read from input nodes.</remarks>
    /// <returns>The output.</returns>
    public abstract NodeOutput<TOutput> Compute();
}