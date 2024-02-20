using ComputationalGraph.Core;
using ComputationalGraph.Nodes.Fundamental;

namespace ComputationalGraph.Benchmarks;

public static class GraphBuilder
{
    public static Graph BuildSteppedGraph(int depth, out SourceNode<int> sourceNode)
    {
        Graph graph = new();

        sourceNode = new SourceNode<int>(graph, 0);

        Node<int> node1 = new BenchmarkNode(graph, sourceNode);
        Node<int> node2 = new BenchmarkNode(graph, sourceNode, node1);

        List<Node<int>> nodes = [sourceNode, node1, node2];

        for (int i = 0; i < depth - 3; i++)
        {
            Node<int> node = new BenchmarkNode(graph, nodes[0], nodes[nodes.Count / 2], nodes[^1]);
            nodes.Add(node);
        }

        return graph;
    }
}