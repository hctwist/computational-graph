using ComputationalGraph.Core;
using ComputationalGraph.Nodes.Fundamental;
using FluentAssertions;

namespace ComputationalGraph.Tests.Tests.Nodes.Fundamental;

[TestClass]
public class SourceNodeTests
{
    [TestMethod]
    public void Compute_WithoutInitialOutput_ReturnsNoOutput()
    {
        Graph graph = new();

        SourceNode<int> sourceNode = new(graph);

        graph.Prime();

        sourceNode.Output.HasOutput.Should().Be(false);
    }

    [TestMethod]
    public void Compute_WithInitialOutput_ReturnsCorrectOutput()
    {
        Graph graph = new();

        int output = 8;
        SourceNode<int> sourceNode = new(graph, output);

        graph.Prime();

        sourceNode.Output.Value.Should().Be(output);
    }
}