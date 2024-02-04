namespace ComputationalGraph.Exceptions;

/// <summary>
/// An exception thrown when trying to access a node's output when it has none.
/// </summary>
public class NoOutputValueException : Exception
{
    public NoOutputValueException(string message) : base(message)
    {
    }
}