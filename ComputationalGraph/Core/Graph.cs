using ComputationalGraph.Exceptions;
using ComputationalGraph.Observing;

namespace ComputationalGraph.Core;

/// <summary>
/// A graph controlling building and firing paths of nodes.
/// </summary>
public class Graph
{
    /// <summary>
    /// Occurs when the graph state changes.
    /// </summary>
    public event Action<GraphState>? StateChanged;

    /// <summary>
    /// Occurs when a node is primed.
    /// This is called immediately as the node is primed.
    /// </summary>
    public event OnNodeFired? NodePrimed;

    /// <summary>
    /// Occurs when a node is fired.
    /// This is called immediately as the node is fired.
    /// </summary>
    public event OnNodeFired? NodeFired;

    /// <summary>
    /// Occurs after nodes have been fired.
    /// This is called after all nodes in a single action have been fired. For example:
    /// <list type="bullet">
    /// <item>After a batch</item>
    /// <item>After all nodes have been primed</item>
    /// <item>After all nodes in a path have been fired</item>
    /// </list>
    /// </summary>
    public event Action? NodesFired;

    /// <summary>
    /// The graph version. This increments every time an operation causes nodes to be fired.
    /// </summary>
    internal int Version;

    /// <summary>
    /// All nodes in the graph.
    /// </summary>
    private readonly HashSet<GraphNode> allNodes;

    /// <summary>
    /// The fire path.
    /// </summary>
    private readonly FirePath firePath;

    /// <summary>
    /// The illegal node inspector.
    /// </summary>
    private readonly NodeInspector nodeInspector;

    /// <summary>
    /// The current batch of nodes to fire.
    /// </summary>
    private readonly HashSet<GraphNode> batchedNodes;

    /// <summary>
    /// The graph state.
    /// </summary>
    private GraphState state;

    /// <summary>
    /// Creates a new <see cref="Graph"/>.
    /// </summary>
    public Graph()
    {
        Version = 0;

        allNodes = new HashSet<GraphNode>();
        firePath = new FirePath();
        nodeInspector = new NodeInspector();
        batchedNodes = new HashSet<GraphNode>();

        state = GraphState.Building;
    }

    /// <summary>
    /// Gets the current graph state.
    /// </summary>
    public GraphState State
    {
        get => state;
        private set
        {
            state = value;
            StateChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// Gets all nodes registered to this graph.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is still building.</exception>
    public IReadOnlySet<GraphNode> AllNodes
    {
        get
        {
            if (State is GraphState.Building)
            {
                throw new InvalidGraphStateException("Cannot access graph nodes before the graph is built");
            }
            
            return allNodes;
        }
    }

    /// <summary>
    /// Primes the graph. This will pre-compute all node outputs and ready the graph for firing.
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    public void Prime()
    {
        if (State != GraphState.Building)
        {
            throw new InvalidGraphStateException($"Cannot prime a graph whilst in state {State}");
        }

        StartFiring(GraphState.Priming);

        firePath.Resolve();

        foreach (GraphNode node in firePath.ConstantOutputNodes)
        {
            node.PathIndex = null;
            node.Fire();
            NodePrimed?.Invoke(node, GetDisplayOutput(node));
        }

        for (int i = 0; i < firePath.Path.Count; i++)
        {
            GraphNode node = firePath.Path[i];
            node.PathIndex = i;
            node.Fire();
            NodePrimed?.Invoke(node, GetDisplayOutput(node));
        }

        EndFiring();
    }

    /// <summary>
    /// Starts a batch. Any nodes fired within the batch will be combined such that no node is fired twice.
    /// </summary>
    /// <param name="batch">The batch.</param>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    public void Batch(Action batch)
    {
        if (State != GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Cannot start a batch whilst in state {State}");
        }

        State = GraphState.Batching;
        batch();

        StartFiring(GraphState.Firing);

        // All nodes in the batch must have a path index (ie. be on the path)
        int pathStart = batchedNodes.Min(n => n.PathIndex!.Value);

        for (int i = pathStart; i < firePath.Path.Count; i++)
        {
            GraphNode pathNode = firePath.Path[i];

            // Fire a node if it's part of the batch or its inputs were fired
            if (batchedNodes.Contains(pathNode) || pathNode.ShouldFire())
            {
                Fire(pathNode);
            }
        }

        batchedNodes.Clear();
        EndFiring();
    }

    /// <summary>
    /// Adds a node to the graph.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The node's path index, or -1 if it wasn't added to the path.</returns>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    internal void AddNode(GraphNode node)
    {
        if (State != GraphState.Building)
        {
            throw new InvalidGraphStateException($"Cannot add a node to the graph whilst in state {State}");
        }

        nodeInspector.Inspect(node.GetType());

        if (!allNodes.Add(node))
        {
            throw new InvalidOperationException($"Node {node.Name} already added to the graph");
        }

        firePath.Add(node);
    }

    /// <summary>
    /// Fires a path stemming from this node.
    /// </summary>
    /// <param name="node">The node to fire.</param>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    internal void FireFrom(GraphNode node)
    {
        if (node.PathIndex is not int pathIndex)
        {
            throw new InvalidOperationException("Tried to fire from a node that isn't on the fire path");
        }

        if (State == GraphState.Batching)
        {
            batchedNodes.Add(node);
            return;
        }

        if (State != GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Cannot fire a node whilst the graph is in state {State}");
        }

        StartFiring(GraphState.Firing);

        // Fire the source node
        Fire(node);

        // Fire any necessary nodes after the node being fired
        for (int i = pathIndex + 1; i < firePath.Path.Count; i++)
        {
            GraphNode pathNode = firePath.Path[i];

            // Fire a node only if its inputs were fired
            if (pathNode.ShouldFire())
            {
                Fire(pathNode);
            }
        }

        EndFiring();
    }

    /// <summary>
    /// Fires a single node.
    /// </summary>
    /// <param name="node">The node.</param>
    private void Fire(GraphNode node)
    {
        node.Fire();
        NodeFired?.Invoke(node, GetDisplayOutput(node));
    }

    /// <summary>
    /// Performs any necessary actions to start firing.
    /// </summary>
    /// <param name="state">The state that </param>
    private void StartFiring(GraphState state)
    {
        State = state;
        Version++;
    }

    /// <summary>
    /// Performs any necessary actions to end firing.
    /// </summary>
    private void EndFiring()
    {
        State = GraphState.Idle;
        NodesFired?.Invoke();
    }

    /// <summary>
    /// Gets the display output for a node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The display output.</returns>
    private static NodeOutput<string?> GetDisplayOutput(GraphNode node)
    {
        return node.LastHadOutput ? NodeOutput<string?>.From(node.LastDisplayOutputValue) : NodeOutput<string?>.Nothing();
    }
}