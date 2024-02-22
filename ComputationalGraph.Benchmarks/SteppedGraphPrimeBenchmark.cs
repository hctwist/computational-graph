using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Benchmarks;

[MemoryDiagnoser]
public class SteppedGraphPrimeBenchmark
{
    [Params(10, 100, 1_000, 10_000)]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public int Depth;
    
    [Benchmark]
    public void Prime()
    {
        GraphBuilder.BuildSteppedGraph(Depth, out _).Prime();
    }
}