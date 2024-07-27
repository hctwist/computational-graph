# Computational Graph

Simple framework for specifying and evaluating [computational graphs](https://en.wikipedia.org/wiki/Directed_graph) in
C#.

## The Purpose

Large, multi-step computations with complex dependencies are relatively difficult to manage safely and efficiently.
Building computations inside the graph framework gives you a few main benefits:

- Safe dependencies - nodes are guaranteed to re-compute if their inputs change and this will propagate through the
  graph
- Computation is optimised and isolated to the subgraphs in which a dependency changed
- A node based architecture lends itself to being easily testable

## Usage

Let's create a graph representing the simple equation:

```math
(x + y) * z
```

For this we first need a graph:

```csharp
Graph graph = new();
```

We need 'parameter' nodes to represent `x`, `y` and `z`:

```csharp
SourceNode<int> x = new(graph);
SourceNode<int> y = new(graph);
SourceNode<int> z = new(graph);
```

Finally let's create two nodes to represent the addition step and the multiplication step:

```csharp
AddNode<int> addition = new(graph, x, y);
MultiplyNode<int> multiplication = new(graph, addition, z);
```

*Note the dependency hierarchy we've created here. The addition node depends on `x` and `y`
whereas the multiplication node depends on `addition` and `z`.*

Now all that remains is to compute the graph, which is done by firing our 'parameter' nodes, and read the output:

```csharp
graph.Prime();

x.Fire(1);
y.Fire(2);
z.Fire(3);

multiplication.Output; // 9, ie. (1 + 2) * 3
```

There is an unexplained step here (`graph.Prime()`) which is covered later in [Priming](#priming).

# The Basics

## Graph

The graph is the entry point in which nodes must be added. It's responsible for maintaining the dependencies between
nodes (the edges) and calculating optimal paths in which nodes can be computed.

### Priming

Before a node can be fired, every node has to be computed for initial values. This step is called priming and is done
via the `Graph.Prime` method:

```csharp
graph.Prime();
```

Priming **must** be done before firing any nodes, and nodes **cannot** be added to a graph after it has been primed.

### Firing

When a node is fired, the graph determines a path to fire nodes. This is constructed based on the dependencies between
nodes and guarantees that all inputs have been fired before dependents are computed.

### Short Circuiting

If a node outputs nothing, then the fire path will short circuit and all dependent nodes in the path will also output
nothing. It follows that if any of a node's inputs output nothing, it will not be computed.

## Nodes

Nodes are the building blocks of computation, as seen in [Usage](#usage).
They can be arbitrarily simple (a single operation: addition, condition etc.) or they can represent an entire algorithm.

### Specifying Nodes

Although some simple nodes come in the framework, the real power comes from creating custom nodes.

Every node must inherit from the `Node<TOutput>` class, where `TOutput` is the output type of the node being created.
Take this `TripleNode` as an example, which takes in a single integer and multiplies it by three:

```csharp
public class TripleNode : Node<int>
{
    private readonly InputNode<int> input;

    public TripleNode(Graph graph, Node<int> input) : base(graph)
    {
        this.input = Input(input);
    }

    public override NodeOutput<int> Compute()
    {
        return input.Value * 3;
    }
}
```

When a node is constructed, it is required to provide a graph which it will be automatically added to.

### Dependencies

Node dependencies are created by defining inputs. In the constructor of a node, its inputs should be registered via
the `Input` method as shown here:

```csharp
public TripleNode(Graph graph, Node<int> input) : base(graph)
{
    this.input = Input(input);
}
```

A node is not permitted to be a direct field of another node, instead a reference to the `InputNode<TOutput>` returned
by `Input` can be held, which provides the input node's output during `Compute`.

### Compute

Every node represents a computation, which is defined in the `Compute` method. This is the only place in which input
values can be safely accessed.

```csharp
public override NodeOutput<int> Compute()
{
    return input.Value * 3;
}
```

### Node Output

As seen [above](#compute), the `Compute` method doesn't just return a `TOutput`, but a `NodeOutput<TOutput>`. This
represents a dual state facilitating nodes which want to output nothing, short circuiting the current fire path.

To output something, either an explicit `NodeOutput<TOutput>` can be created or an implicit cast can be used:

```csharp
return NodeOutput<int>.From(0);
return 0; // Implicitly casted
```

Outputting nothing can similarly be done by creating an explicit `NodeOutput<TOutput>`,
or using the shorthand `Nothing` method:

```csharp
return NodeOutput<int>.Nothing();
return Nothing();
```

### Fallback Output

As noted in [short circuiting](#short-circuiting), nodes of which their inputs output nothing do not compute, but they
can output a fallback. Nodes can specify a fallback value or node via the secondary constructor:

```csharp
public NodeWithFallback(Graph graph, Node<int> fallbackNode) : base(graph, fallbackNode)
{
}

public NodeWithFallback(Graph graph, int fallbackValue) : base(graph, fallbackValue)
{
}
```

___

# The Rest

## Framework Nodes

### Fundamental Nodes

There are a set of core fundamental nodes which work closely with the framework and cannot be replicated. These are
present in the `Nodes.Fundamental` namespace. It is safe to assume that any nodes outside of this namespace can be
created solely with public constructs.

#### Source Node

The source node (`SourceNode<TOutput>`) is the only node that can be explicitly fired, and therefore represents the
only node that can trigger the graph.

```csharp
SourceNode<int> sourceNode = new(graph);
```

The node can be fired via `Fire`:

```csharp
sourceNode.Fire(8);
sourceNode.Fire(NodeOutput<int>.Nothing());
```

#### Constant Node

The constant node is namely a node that outputs a fixed value.

```csharp
ConstantNode<int> constantNode = new(graph, 8);
constantNode.Output; // 8
```

Although a custom constant node could be created, the included `ConstantNode<TOutput>` is optimised as a special case.

### Building Block Nodes

There are a few nodes in the `Nodes.BuildingBlocks` namespace that simplify specifying other nodes.

This is an example of an integer addition node created with `AggregateNode<TInput, TOutput>`:

```csharp
public class AddNode : AggregateNode<int, int>
{
    public AddNode(Graph graph, params Node<int>[] operands) : base(graph, operands)
    {
    }

    protected override NodeOutput<int> Compute(IEnumerable<int> inputs)
    {
        return inputs.Sum();
    }
}
```

### General Nodes

#### Fallback Node

The fallback node (`FallbackNode<TOutput>`) provides a fallback mechanism with another node. For example:

```csharp
ConstantNode<int> fallbackValueNode = new(graph, 8);

SourceNode<int> inputNode = new(graph);
FallbackNode<int> guardedInputNode = new(graph, inputNode, fallbackValueNode);
FallbackNode<int> guardedInputNode = inputNode.WithFallback(fallbackValueNode);
```

will output the following:

```csharp
inputNode.Fire(NodeOutput<int>.Nothing());
guardedInputNode.Output; // 8

inputNode.Fire(0);
guardedInputNode.Output; // 0
```

## Batching

Every time a source node is fired, the graph fires all necessary nodes immediately. This can be
wasteful if wanting to fire multiple nodes at once. To solve this you can batch fire nodes at the graph level via the
`Batch` method:

```csharp
graph.Batch(
    () =>
    {
        node0.Fire(0);
        node1.Fire(0);
    });
```

Any nodes fired within the action passed will be batched. No nodes will be computed more than once per batch.

## Events

### Fired (Node)

Nodes have a single event that can be subscribed to: `Fired`. This occurs immediately as the node is fired
(potentially before a fire path is complete) and provides the output of the node:

```csharp
node.Fired += (node, output) => DoSomething(output);
```

### Primed and Fired (Graph)

`NodePrimed` and `NodeFired` are events that occur when a node is primed or fired, and operate on the graph level.
These can be used as an alternative to the `Fired` event directly on nodes:

```csharp
graph.NodePrimed += (node, output) => Console.WriteLine($"Primed node {node.Name}. Output = {output}");
graph.NodeFired += (node, output) => Console.WriteLine($"Fired node {node.Name}. Output = {output}");
```

### Nodes Fired (Graph)

The `NodesFired` event on the graph occurs once per fire path.
This is a good alternative to individual events to batch external updates based on node output.
This can be used in conjunction with `Node<TOutput>.WasFired` to ignore nodes that weren't fired in the path:

```csharp
graph.NodesFired += () =>
{
    if (node.WasFired)
    {
        ...
    }
};
```

## Rules

There are a few rules that should be followed in order to ensure the graph computes as intended.

### Nodes as Fields

Nodes should not be fields of other nodes, directly or indirectly. Output of other nodes should only be consumed via
[input nodes](#dependencies).

The graph attempts to detect illegal fields and will throw an exception when the illegal node is added, however
this doesn't cover every case so care should still be taken.

### Graph States

A graph moves between states throughout its lifetime, the states being:

- Building
- Priming
- Idle
- Firing
- Batching

Most operations require the graph to be in a particular state (most of which should be intuitive). Some examples of
violating actions are:

- Firing nodes whilst the graph is not idle (or batching)
- Adding nodes whilst the graph is not building
- Accessing node output whilst the graph is not idle (aside from via input nodes in `Compute`)

### Cycles

Graph cycles are not permitted and will be checked for when the graph is priming.

# Performance

## Speed

Firing nodes is lightweight. Paths are pre-constructed when the graph is primed and guarantee that:

- No node is computed twice
- Nodes will be computed only if their inputs have been

## Allocations

Firing nodes in the graph is considered to be zero-allocation (no heap allocations), with the exception of when a
handler is attached to the following graph-level events:

- `Graph.NodePrimed`
- `Graph.NodeFired`

as these both construct string outputs of the nodes. The other node firing events do not provide string output so are safe.

*Note that there are no allocation guarantees in the build or prime stage of the graph.*

## Benchmarks

The overhead in the framework is slight so should be dwarfed by compute logic in the nodes, but here are some simple
benchmark results on firing empty nodes to get an idea of usage with a large number of nodes:

| Nodes |         Mean |     Error |    StdDev | Allocated |
|-------|-------------:|----------:|----------:|----------:|
| 10    |     140.7 ns |   0.79 ns |   0.74 ns |         - |
| 100   |   1,444.9 ns |   6.30 ns |   5.89 ns |         - |
| 1000  |  15,085.9 ns | 280.22 ns | 262.12 ns |         - |
| 10000 | 154,435.1 ns | 957.34 ns | 895.49 ns |         - |
