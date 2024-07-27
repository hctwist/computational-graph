namespace ComputationalGraph.Exceptions;

/// <summary>
/// An exception thrown when a cycle in the graph is detected.
/// </summary>
public class GraphCycleException : Exception
{
    public GraphCycleException(string message) : base(message)
    {
    }
}