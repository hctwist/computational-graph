namespace ComputationalGraph.Core;

public class TestNode : Node<int>
{
    /// <inheritdoc />
    public TestNode(Graph graph, int fallback) : base(graph, fallback)
    {
    }

    /// <inheritdoc />
    public TestNode(Graph graph, Node<int> fallbackNode) : base(graph, fallbackNode)
    {
    }

    /// <inheritdoc />
    public override NodeOutput<int> Compute()
    {
        throw new NotImplementedException();
    }
}