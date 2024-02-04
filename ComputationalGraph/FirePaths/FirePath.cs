using System.Collections;
using ComputationalGraph.Core;

namespace ComputationalGraph.FirePaths;

/// <summary>
/// A node fire path for a single source node.
/// </summary>
internal class FirePath : IEnumerable<Node>
{
    /// <summary>
    /// The queue of nodes to be processed.
    /// This should be cleared before use.
    /// </summary>
    private readonly Queue<Node> queue;

    /// <summary>
    /// A lookup between nodes and their position in the path (<see cref="path"/>).
    /// </summary>
    private readonly Dictionary<Node, LinkedListNode<Node>> lookup;

    /// <summary>
    /// The current path.
    /// This should be cleared before use.
    /// </summary>
    private readonly LinkedList<Node> path;

    /// <summary>
    /// Creates a new <see cref="FirePath"/>.
    /// </summary>
    public FirePath()
    {
        queue = new Queue<Node>();
        lookup = new Dictionary<Node, LinkedListNode<Node>>();
        path = new LinkedList<Node>();
    }

    /// <summary>
    /// Populates this path from a source node.
    /// </summary>
    /// <param name="sourceNode">The source node being fired.</param>
    public void Populate(Node sourceNode)
    {
        queue.Clear();
        lookup.Clear();
        path.Clear();

        queue.Enqueue(sourceNode);

        while (queue.TryDequeue(out Node? queuedNode))
        {
            Add(queuedNode);

            foreach (Node dependent in queuedNode.Dependents)
            {
                queue.Enqueue(dependent);
            }
        }
    }

    /// <summary>
    /// Adds a single node to the path.
    /// </summary>
    /// <param name="node">The node.</param>
    private void Add(Node node)
    {
        if (lookup.Remove(node, out LinkedListNode<Node>? pathNode))
        {
            path.Remove(pathNode);
        }

        lookup.Add(node, path.AddLast(node));
    }

    /// <inheritdoc />
    public IEnumerator<Node> GetEnumerator()
    {
        return path.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}