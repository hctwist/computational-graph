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
    /// The last output of this node.
    /// </summary>
    internal NodeOutput<TOutput> LastOutput;

    /// <inheritdoc />
    public override string Name { get; }

    /// <inheritdoc />
    internal override int? PathIndex { get; }

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
    private readonly Node<TOutput>? fallbackNode;

    /// <summary>
    /// Creates a new <see cref="Node{TOutput}"/>.
    /// </summary>
    /// <param name="graph">The graph to add this node to.</param>
    /// <param name="fallback">The fallback output.</param>
    protected Node(Graph graph, TOutput fallback) : this(graph, new ConstantNode<TOutput>(graph, fallback))
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

        int? pathIndex = graph.AddNode(this);
        PathIndex = pathIndex;
        
        Name = $"{GetType().Name}[{pathIndex}]";

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
            if (Graph.State is GraphState.Firing or GraphState.Priming)
            {
                throw new InvalidGraphStateException($"Attempted to read output whilst the graph was {Graph.State}");
            }

            return LastOutput;
        }
    }

    /// <inheritdoc/>
    public override bool WasFired => version == Graph.Version;

    /// <inheritdoc />
    internal sealed override bool LastHadOutput => LastOutput.HasOutput;

    /// <inheritdoc />
    internal sealed override string LastDisplayOutput => LastOutput.Value?.ToString() ?? string.Empty;

    /// <inheritdoc />
    internal override bool ShouldFire()
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

        // Make sure the node is before this one on the path
        if (inputNode.PathIndex is int inputNodePathIndex && inputNodePathIndex >= PathIndex)
        {
            throw new InvalidNodeInputException(
                $"""
                 Node {inputNode} cannot be added as an input to node {Name} as it doesn't sit before it in the graph.
                 This could be because the node was created whilst constructing this node, or this is being called post-construction
                 """);
        }

        if (!inputs.Add(inputNode))
        {
            throw new InvalidNodeInputException($"Node {inputNode.Name} has already been added as an input of {Name}");
        }

        return new NodeInput<TInput>(inputNode);
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