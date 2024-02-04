using ComputationalGraph.Exceptions;
using ComputationalGraph.FirePaths;
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
    private readonly HashSet<Node> nodes;

    /// <summary>
    /// The node inspector.
    /// </summary>
    private readonly NodeInspector inspector;

    /// <summary>
    /// The fire path for a single node being fired.
    /// </summary>
    private readonly FirePath singleFirePath;

    /// <summary>
    /// The fire path for a batch of nodes being fired.
    /// </summary>
    private readonly BatchFirePath batchFirePath;

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
        
        nodes = new HashSet<Node>();
        inspector = new NodeInspector();
        singleFirePath = new FirePath();
        batchFirePath = new BatchFirePath();

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

        HashSet<Node> nodesPrimed = new();
        Stack<Node> nodesToPrime = new(nodes);
        
        while (nodesToPrime.TryPop(out Node? nodeToPrime))
        {
            if (nodesPrimed.Contains(nodeToPrime))
            {
                continue;
            }

            if (nodeToPrime.Inputs.All(inputNode => nodesPrimed.Contains(inputNode)))
            {
                nodeToPrime.Fire();
                NodePrimed?.Invoke(nodeToPrime, GetDisplayOutput(nodeToPrime));

                nodesPrimed.Add(nodeToPrime);
                continue;
            }

            nodesToPrime.Push(nodeToPrime);

            foreach (Node inputNode in nodeToPrime.Inputs)
            {
                if (!nodesPrimed.Contains(inputNode))
                {
                    nodesToPrime.Push(inputNode);
                }
            }
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
        
        Fire(batchFirePath);
        batchFirePath.Clear();

        EndFiring();
    }

    /// <summary>
    /// Adds a node to the graph.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>A unique identifier for the node.</returns>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    internal int AddNode(Node node)
    {
        if (State != GraphState.Building)
        {
            throw new InvalidGraphStateException($"Cannot add a node to the graph whilst in state {State}");
        }

        inspector.Inspect(node);

        if (!nodes.Add(node))
        {
            throw new InvalidOperationException($"Node {node.Name} already added to the graph");
        }

        return nodes.Count - 1;
    }

    /// <summary>
    /// Fires a path stemming from this node.
    /// </summary>
    /// <param name="node">The node to fire.</param>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    internal void Fire(Node node)
    {
        if (State == GraphState.Batching)
        {
            batchFirePath.Add(node);
            return;
        }
        
        if (State != GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Cannot fire a node whilst the graph is in state {State}");
        }

        StartFiring(GraphState.Firing);

        singleFirePath.Populate(node);
        Fire(singleFirePath);

        EndFiring();
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
    /// Fires a collection of nodes.
    /// </summary>
    /// <remarks>This fires individual nodes only, no paths are formed.</remarks>
    /// <param name="nodes">The nodes to fire.</param>
    private void Fire(IEnumerable<Node> nodes)
    {
        foreach (Node pathNode in nodes)
        {
            pathNode.Fire();
            NodeFired?.Invoke(pathNode, GetDisplayOutput(pathNode));
        }
    }

    /// <summary>
    /// Gets the display output for a node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The display output.</returns>
    private static NodeOutput<string> GetDisplayOutput(Node node)
    {
        return node.LastHadOutput ? NodeOutput<string>.From(node.LastDisplayOutput) : NodeOutput<string>.Nothing();
    }
}