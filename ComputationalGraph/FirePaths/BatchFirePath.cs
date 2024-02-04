using System.Collections;
using ComputationalGraph.Core;

namespace ComputationalGraph.FirePaths;

/// <summary>
/// A node fire path for a batch.
/// </summary>
internal class BatchFirePath : IEnumerable<Node>
{
    /// <summary>
    /// A pre-allocated single fire path.
    /// </summary>
    private readonly FirePath singleFirePath;

    /// <summary>
    /// All nodes in the path.
    /// </summary>
    private readonly HashSet<Node> nodes;

    /// <summary>
    /// The current path.
    /// </summary>
    private readonly LinkedList<Node> path;

    /// <summary>
    /// Creates a new <see cref="BatchFirePath"/>.
    /// </summary>
    public BatchFirePath()
    {
        singleFirePath = new FirePath();
        nodes = new HashSet<Node>();
        path = new LinkedList<Node>();
    }

    /// <summary>
    /// Adds a node to the fire path.
    /// </summary>
    /// <param name="node">The node.</param>
    public void Add(Node node)
    {
        if (nodes.Contains(node))
        {
            return;
        }

        singleFirePath.Populate(node);

        LinkedListNode<Node>? insertionPoint = null;

        foreach (Node singleFirePathNode in singleFirePath)
        {
            if (!nodes.Add(singleFirePathNode))
            {
                continue;
            }

            insertionPoint = insertionPoint is null ? path.AddFirst(singleFirePathNode) : path.AddAfter(insertionPoint, singleFirePathNode);
        }
    }

    /// <summary>
    /// Clears the path.
    /// </summary>
    public void Clear()
    {
        nodes.Clear();
        path.Clear();
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