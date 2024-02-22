using BenchmarkDotNet.Running;
using ComputationalGraph.Benchmarks;

BenchmarkRunner.Run([typeof(SteppedGraphPrimeBenchmark), typeof(SteppedGraphFireBenchmark)]);