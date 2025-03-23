using ComputationalGraph.Core;

namespace ComputationalGraph.Nodes.Fundamental;

/// <summary>
/// Node that 'pulses' with a source value.
/// When the source node is fired, this node will fire twice, once with the source value and once with the default value.
/// </summary>
/// <typeparam name="TOutput">The output type.</typeparam>
public sealed class PulseNode<TOutput> : Node<TOutput>
{
    private readonly Func<bool> shouldPulse;
    private readonly NodeInput<TOutput> sourceInputNode;

    private readonly NodeOutput<TOutput> defaultOutput;

    /// <inheritdoc />
    public PulseNode(Graph graph, Node<TOutput> sourceNode, NodeOutput<TOutput> defaultOutput) : base(graph)
    {
        shouldPulse = () => sourceNode.WasFired;
        sourceInputNode = Input(sourceNode);

        this.defaultOutput = defaultOutput;
    }

    /// <inheritdoc />
    protected override NodeOutput<TOutput> Compute()
    {
        if (shouldPulse())
        {
            Graph.RequestRefire(this);
            return sourceInputNode.Value;
        }
        else
        {
            return defaultOutput;
        }
    }
}

public static class PulseNodeExtensions
{
    public static Node<TOutput> Pulse<TOutput>(this Node<TOutput> sourceNode, NodeOutput<TOutput> defaultOutput)
    {
        return new PulseNode<TOutput>(sourceNode.Graph, sourceNode, defaultOutput);
    }
}