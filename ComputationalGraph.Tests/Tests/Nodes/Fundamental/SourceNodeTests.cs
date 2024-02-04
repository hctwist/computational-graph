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
        SourceNode<int> sourceNode = new(new Graph());
        sourceNode.Compute().HasOutput.Should().Be(false);
    }
    
    [TestMethod]
    public void Compute_WithInitialOutput_ReturnsCorrectOutput()
    {
        int output = 8;
        SourceNode<int> sourceNode = new(new Graph(), output);
        sourceNode.Compute().Value.Should().Be(output);
    }
}