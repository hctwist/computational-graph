using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Benchmarks;

[MemoryDiagnoser]
public class SteppedGraphBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public int Depth;

    private SourceNode<int> sourceNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        GraphBuilder.BuildSteppedGraph(Depth, out sourceNode).Prime();
    }

    [Benchmark]
    public void Benchmark()
    {
        sourceNode.Fire(0);
    }
}