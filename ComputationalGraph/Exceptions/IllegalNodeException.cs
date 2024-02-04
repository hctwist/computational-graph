namespace ComputationalGraph.Exceptions;

/// <summary>
/// An exception thrown when a node is invalid.
/// </summary>
public class IllegalNodeException : Exception
{
    public IllegalNodeException(string message) : base(message)
    {
    }
}