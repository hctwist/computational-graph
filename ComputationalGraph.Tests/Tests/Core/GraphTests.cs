using ComputationalGraph.Core;
using ComputationalGraph.Nodes.Fundamental;
using ComputationalGraph.Tests.Nodes;
using FluentAssertions;

namespace ComputationalGraph.Tests.Tests.Core;

[TestClass]
public class GraphTests
{
    [TestMethod]
    public void Prime_NodesWithExpectedOutput_AllHaveOutput()
    {
        Graph graph = new();

        SourceNode<int> node0 = new(graph, 0);
        PassthroughNode<int> node1 = new(graph, node0);
        PassthroughNode<int> node2 = new(graph, node0);
        PassthroughNode<int> node3 = new(graph, node2);
        SourceNode<int> node4 = new(graph, 0);
        
        List<Node<int>> nodes = [node0, node1, node2, node3, node4];

        graph.Prime();
        
        nodes.Select(node => node.Output.HasOutput).Should().AllBeEquivalentTo(true);
    }
    
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(10)]
    [DataRow(100)]
    [TestMethod]
    public void Fire_NodeChain_FiresInOrder(int nodeCount)
    {
        Graph graph = new();

        SourceNode<int> sourceNode = new(graph, 0);
        List<Node<int>> nodes = [sourceNode];

        for (int i = 0; i < nodeCount - 1; i++)
        {
            nodes.Add(new PassthroughNode<int>(graph, nodes[^1]));
        }
        
        graph.Prime();

        List<GraphNode> firedNodes = new();

        graph.NodeFired += (node, _) => firedNodes.Add(node);
        
        sourceNode.Fire(0);

        firedNodes.Should().Equal(nodes);
    }
}