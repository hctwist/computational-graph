using ComputationalGraph.Core;

namespace ComputationalGraph.Benchmarks;

/// <summary>
/// A lightweight node used to benchmark the graph without the overhead of computation.
/// </summary>
public class BenchmarkNode : Node<int>
{
    /// <inheritdoc />
    public BenchmarkNode(Graph graph, params Node<int>[] inputNodes) : base(graph)
    {
        foreach (Node<int> inputNode in inputNodes)
        {
            Input(inputNode);
        }
    }

    /// <inheritdoc />
    public override NodeOutput<int> Compute()
    {
        return 0;
    }
}