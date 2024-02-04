namespace ComputationalGraph.Exceptions;

/// <summary>
/// An exception thrown when a node input is invalid.
/// </summary>
public class InvalidNodeInputException : Exception
{
    public InvalidNodeInputException(string message) : base(message)
    {
    }
}