using ComputationalGraph.Exceptions;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Core;

/// <summary>
/// A graph node.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public abstract class Node<TOutput> : GraphNode
{
    /// <summary>
    /// Occurs when the node is fired.
    /// </summary>
    /// <remarks>This is called immediately as the node is fired.</remarks>
    public event Action<NodeOutput<TOutput>>? Fired;

    /// <summary>
    /// The graph this node belongs to.
    /// </summary>
    public readonly Graph Graph;

    /// <summary>
    /// Gets the last output of this node.
    /// </summary>
    /// <remarks>This is <see cref="NodeOutput{TOutput}.Nothing"/> before the node is fired.</remarks>
    internal NodeOutput<TOutput> LastOutput { get; private set; }

    /// <summary>
    /// The node's version, or null if the node hasn't been fired.
    /// </summary>
    private int? version;

    /// <summary>
    /// The node's inputs.
    /// </summary>
    private readonly HashSet<GraphNode> inputs;

    /// <summary>
    /// The fallback node, or null if there is no fallback.
    /// </summary>
#pragma warning disable CGRAPH0001
    private readonly Node<TOutput>? fallbackNode;
#pragma warning restore CGRAPH0001

    /// <summary>
    /// Creates a new <see cref="Node{TOutput}"/>.
    /// </summary>
    /// <param name="graph">The graph to add this node to.</param>
    /// <param name="fallback">The fallback output.</param>
    protected Node(Graph graph, NodeOutput<TOutput> fallback) : this(graph, new ConstantNode<TOutput>(graph, fallback))
    {
    }

    /// <summary>
    /// Creates a new <see cref="Node{TOutput}"/>.
    /// </summary>
    /// <param name="graph">The graph to add this node to.</param>
    /// <param name="fallbackNode">The fallback node, or null if no fallback is to be specified.</param>
    protected Node(Graph graph, Node<TOutput>? fallbackNode = null)
    {
        Graph = graph;

        LastOutput = Nothing();

        graph.AddNode(this);

        version = null;

        inputs = new HashSet<GraphNode>();

        this.fallbackNode = fallbackNode;
        if (fallbackNode is not null)
        {
            Input(fallbackNode);
        }
    }

    /// <summary>
    /// Gets the node's output.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    public NodeOutput<TOutput> Output
    {
        get
        {
            EnsureOutputCanBeAccessed();
            return LastOutput;
        }
    }

    /// <inheritdoc/>
    public sealed override NodeOutput<string?> DisplayOutput
    {
        get
        {
            EnsureOutputCanBeAccessed();
            return LastHadOutput ? LastDisplayOutputValue : NodeOutput<string?>.Nothing();
        }
    }
    
    /// <inheritdoc/>
    public sealed override bool WasFired => version == Graph.Version;

    /// <inheritdoc />
    public sealed override IReadOnlySet<GraphNode> Inputs => inputs;

    /// <inheritdoc />
    internal sealed override bool LastHadOutput => LastOutput.HasOutput;

    /// <inheritdoc />
    internal sealed override string LastDisplayOutputValue => LastOutput.Value?.ToString() ?? string.Empty;

    /// <inheritdoc />
    internal sealed override bool ShouldFire()
    {
        foreach (GraphNode inputNode in inputs)
        {
            if (inputNode.WasFired)
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    internal sealed override void Fire()
    {
        LastOutput = DetermineOutput();
        version = Graph.Version;
        Fired?.Invoke(LastOutput);
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
    protected NodeInput<TInput> Input<TInput>(Node<TInput> inputNode)
    {
        if (inputNode.Graph != Graph)
        {
            throw new InvalidNodeInputException($"Tried to add node {inputNode.Name} from a different graph as an input to node {Name}");
        }

        if (Graph.State != GraphState.Building)
        {
            throw new InvalidGraphStateException($"Tried to add node {inputNode.Name} as an input to node {Name} whilst the graph is {Graph.State}");
        }

        if (Primed)
        {
            throw new InvalidNodeInputException($"Tried to add an input to node {Name} after it has been primed");
        }

        if (!inputs.Add(inputNode))
        {
            throw new InvalidNodeInputException($"Node {inputNode.Name} has already been added as an input of {Name}");
        }

        return new NodeInput<TInput>(inputNode);
    }

    /// <summary>
    /// Determines the current output of the node.
    /// </summary>
    /// <returns>The output.</returns>
    private NodeOutput<TOutput> DetermineOutput()
    {
        foreach (GraphNode input in inputs)
        {
            if (input != fallbackNode && !input.LastHadOutput)
            {
                return fallbackNode?.LastOutput ?? Nothing();
            }
        }

        return Compute();
    }

    /// <summary>
    /// Ensures that the graph is in a valid state for output to be accessed.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    private void EnsureOutputCanBeAccessed()
    {
        if (Graph.State is not GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Attempted to read output whilst the graph was {Graph.State}");
        }
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