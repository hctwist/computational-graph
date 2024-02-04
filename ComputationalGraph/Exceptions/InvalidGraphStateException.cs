using ComputationalGraph.Core;

namespace ComputationalGraph.Exceptions;

/// <summary>
/// An exception thrown when an operation is performed on a <see cref="Graph"/> whilst it is in an invalid state.
/// </summary>
public class InvalidGraphStateException : Exception
{
    public InvalidGraphStateException(string message) : base(message)
    {
    }
}