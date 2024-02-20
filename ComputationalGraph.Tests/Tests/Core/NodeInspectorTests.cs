using System.Reflection;
using ComputationalGraph.Core;
using FluentAssertions;

namespace ComputationalGraph.Tests.Tests.Core;

[TestClass]
public class NodeInspectorTests
{
    private readonly NodeInspector inspector;

    public NodeInspectorTests()
    {
        inspector = new NodeInspector();
    }
    
    [TestMethod]
    public void Inspect_AllNodes_Valid()
    {
        foreach (Type type in Assembly.GetAssembly(typeof(Graph)).Types().ThatDeriveFrom<GraphNode>())
        {
            type.Invoking(t => inspector.Inspect(t)).Should().NotThrow();
        }
    }
}