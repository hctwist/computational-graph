﻿using ComputationalGraph.Exceptions;
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
    /// Occurs when a node is fired.
    /// This is called immediately as the node is fired.
    /// </summary>
    /// <remarks>This also occurs when priming.</remarks>
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
    /// Occurs when the graph is primed.
    /// </summary>
    public event Action? Primed;

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
    /// Nodes to refire.
    /// </summary>
    private readonly HashSet<GraphNode> refireNodes;

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

        allNodes = [];
        firePath = new FirePath();
        nodeInspector = new NodeInspector();
        batchedNodes = [];
        refireNodes = [];

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
            GraphState previousState = state;

            state = value;

            if (previousState != state)
            {
                StateChanged?.Invoke(value);
            }
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
    /// <remarks>
    /// Note that node outputs will not be re-computed if they have been primed already (from a previous call to <see cref="Prime"/>).
    /// </remarks>
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
            PrimeSingleIfNotAlready(node);
        }

        for (int i = 0; i < firePath.Path.Count; i++)
        {
            GraphNode node = firePath.Path[i];
            PrimeSingleIfNotAlready(node);
            node.PathIndex = i;
        }

        EndFiring();
        
        Primed?.Invoke();
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

        Fire(batchedNodes);

        batchedNodes.Clear();
        RefireAndEndFiring();
    }

    /// <summary>
    /// Place the graph back into the <see cref="GraphState.Building"/> state so that more nodes can be added.
    /// Before firing any nodes, the graph must be primed again via <see cref="Prime"/>. 
    /// </summary>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    public void Disengage()
    {
        if (State != GraphState.Idle)
        {
            throw new InvalidGraphStateException($"Cannot rebuild the graph whilst in state {State}");
        }

        State = GraphState.Building;
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
    internal void Fire(GraphNode node)
    {
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
        FireSingle(node);

        // Fire any necessary nodes after the node being fired
        for (int i = node.PathIndex!.Value + 1; i < firePath.Path.Count; i++)
        {
            GraphNode pathNode = firePath.Path[i];

            // Fire a node only if its inputs were fired
            if (pathNode.ShouldFire())
            {
                FireSingle(pathNode);
            }
        }

        RefireAndEndFiring();
    }

    /// <summary>
    /// Request that a node gets refired.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <exception cref="InvalidGraphStateException">Thrown if the graph is in an invalid state.</exception>
    internal void RequestRefire(GraphNode node)
    {
        if (State is not (GraphState.Firing or GraphState.Priming))
        {
            throw new InvalidGraphStateException($"Cannot refire a node whilst the graph is in state {State}");
        }

        refireNodes.Add(node);
    }

    /// <summary>
    /// Fires a set of nodes.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    private void Fire(ISet<GraphNode> nodes)
    {
        // All nodes in the set must have a path index (ie. be on the path)
        int pathStart = nodes.Min(n => n.PathIndex!.Value);

        for (int i = pathStart; i < firePath.Path.Count; i++)
        {
            GraphNode pathNode = firePath.Path[i];

            // Fire a node if it's part of the batch or its inputs were fired
            if (nodes.Contains(pathNode) || pathNode.ShouldFire())
            {
                FireSingle(pathNode);
            }
        }
    }

    /// <summary>
    /// Primes a single node if it hasn't already been primed.
    /// </summary>
    /// <param name="node">The node.</param>
    private void PrimeSingleIfNotAlready(GraphNode node)
    {
        if (node.Primed)
        {
            return;
        }
        
        FireSingle(node);
        node.Primed = true;
    }

    /// <summary>
    /// Fires a single node.
    /// </summary>
    /// <param name="node">The node.</param>
    private void FireSingle(GraphNode node)
    {
        node.Fire();
        NodeFired?.Invoke(node, node.LastHadOutput ? node.LastOutputValue : NodeOutput<object?>.Nothing());
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
    /// Refires any nodes if necessary, then ends firing.
    /// </summary>
    private void RefireAndEndFiring()
    {
        if (refireNodes.Count > 0)
        {
            StartFiring(GraphState.Refiring);
            Fire(refireNodes);
            refireNodes.Clear();
        }

        EndFiring();
    }

    /// <summary>
    /// Performs any necessary actions to end firing.
    /// </summary>
    private void EndFiring()
    {
        State = GraphState.Idle;
        NodesFired?.Invoke();
    }
}