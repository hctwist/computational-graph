using ComputationalGraph.Core;

namespace ComputationalGraph.Analyzers.Sample;

public class SampleNode : Node<int>
{
#pragma warning disable CGRAPH0001
    private readonly Node<int> input;
#pragma warning restore CGRAPH0001
    
    /// <inheritdoc />
    public SampleNode(Graph graph, Node<int> input) : base(graph)
    {
        this.input = input;
    }

    /// <inheritdoc />
    protected override NodeOutput<int> Compute()
    {
        return input.Output;
    }
}