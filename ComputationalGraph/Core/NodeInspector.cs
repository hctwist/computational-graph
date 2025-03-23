using System.Reflection;
using ComputationalGraph.Exceptions;

namespace ComputationalGraph.Core;

/// <summary>
/// Examines nodes for legality.
/// </summary>
public class NodeInspector
{
    /// <summary>
    /// The processing stack.
    /// This should be cleared before use.
    /// </summary>
    private readonly Stack<Type> stack;

    /// <summary>
    /// A pre-allocated set of encountered types that can be skipped during inspection (this can be used to avoid cycles).
    /// This should be cleared before use.
    /// </summary>
    private readonly HashSet<Type> encounteredTypes;
    
    /// <summary>
    /// Types that have already been inspected and were deemed safe.
    /// </summary>
    private readonly HashSet<Type> safeTypes;

    /// <summary>
    /// Creates a new <see cref="NodeInspector"/>.
    /// </summary>
    public NodeInspector()
    {
        stack = new Stack<Type>();
        encounteredTypes = [];
        safeTypes = [];
    }

    /// <summary>
    /// Inspects a node.
    /// </summary>
    /// <param name="nodeType">The node.</param>
    /// <exception cref="IllegalNodeException">Thrown if the node is deemed illegal.</exception>
    // TODO Check for capturing nodes in closures
    public void Inspect(Type nodeType)
    {
        stack.Clear();
        encounteredTypes.Clear();
        
        // Check all direct node fields up to the base node
        foreach (Type nodeField in GetFieldsWithInheritance(nodeType, typeof(Node<>)))
        {
            stack.Push(nodeField);
        }

        while (stack.TryPop(out Type? type))
        {
            if (safeTypes.Contains(type))
            {
                continue;
            }

            if (encounteredTypes.Contains(type))
            {
                continue;
            }

            if (IsInputNode(type))
            {
                continue;
            }

            if (IsNode(type))
            {
                throw new IllegalNodeException($"Node {nodeType.Name} has another node as an eventual field ({type.Name})");
            }

            foreach (Type mentionedType in GetMentionedTypes(type))
            {
                stack.Push(mentionedType);
            }
            
            foreach (Type field in GetFieldsWithInheritance(type))
            {
                stack.Push(field);
            }

            encounteredTypes.Add(type);
        }

        safeTypes.Add(nodeType);

        // Mark all encountered types as safe, as they've now been checked
        foreach (Type encounteredType in encounteredTypes)
        {
            safeTypes.Add(encounteredType);
        }
    }

    /// <summary>
    /// Gets all fields from a type and all of its base types.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="upTo">The base type to search up until (exclusive).</param>
    /// <returns>The fields.</returns>
    private static IEnumerable<Type> GetFieldsWithInheritance(Type type, Type? upTo = null)
    {
        Type? cursor = type;

        while (cursor is not null)
        {
            if ((cursor.IsGenericType ? cursor.GetGenericTypeDefinition() : cursor) == upTo)
            {
                yield break;
            }

            IEnumerable<Type> declaredFields = cursor
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(f => f.FieldType);

            foreach (Type declaredField in declaredFields)
            {
                yield return declaredField;
            }

            cursor = cursor.BaseType;
        }
    }

    /// <summary>
    /// Gets all mentioned types for another type. For example via generics.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The mentioned types (this doesn't include the original type).</returns>
    private static IEnumerable<Type> GetMentionedTypes(Type type)
    {
        if (type.HasElementType)
        {
            yield return type.GetElementType()!;
        }

        if (type.IsGenericType)
        {
            foreach (Type genericArgument in type.GetGenericArguments())
            {
                yield return genericArgument;
            }
        }
    }

    /// <summary>
    /// Determines whether a type is a node.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>True if the type is a node, false otherwise.</returns>
    private static bool IsNode(Type type)
    {
        return type.IsAssignableTo(typeof(GraphNode));
    }

    /// <summary>
    /// Determines whether a type is an input node.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>True if the type is an input node, false otherwise.</returns>
    private static bool IsInputNode(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NodeInput<>);
    }
}