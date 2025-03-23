# Computational Graph Visualisation

Simple extension library to serve as a guide or aid with creating graph visualisations.

## Descriptors

A descriptor represents a piece of structural or stateful information about the graph. There are currently
three descriptors:

- Node descriptors - all nodes in the graph with IDs
- Edge descriptors - unique edges between nodes
- Output descriptors - node output

## The Visualiser

The visualiser is the entry point for accessing descriptors.

To get started create an instance of `GraphVisualiser` for your graph:

```csharp
GraphVisualiser visualiser = new(graph);
```

The visualiser has properties for each descriptor type, which give you [containers](#descriptor-containers) to interact
with descriptors:

```csharp
DescriptorContainer<IReadOnlySet<NodeDescriptor>> nodes = visualiser.Nodes;
DescriptorContainer<IReadOnlySet<EdgeDescriptor>> edges = visualiser.Edges;
DescriptorContainer<IReadOnlySet<OuptutDescriptor>> outputs = visualiser.Outputs;
```

Note that the containers above all actually contain *sets* of descriptors. 

## Descriptor Containers

Descriptor containers provide two methods of accessing their underlying descriptors:

- `Get`
- `Subscribe`

which can be used dependent on whether you want to receive updates in response to changes in the graph.

### Get

The `DescriptorContainer<>.Get` method retrieves the current descriptors (ie. a snapshot).

```csharp
foreach (EdgeDescriptor descriptor in visualiser.Edges.Get())
{
    ...    
}
```

### Subscribe

The `DescriptorContainer<>.Subscribe` method allows subscription to all changes to the current descriptor.
Subscription will always also provide you with an initial snapshot of the descriptors, either when called or when
the graph is ready so there is no need to use `Get` to manually obtain initial state.

```csharp
SourceNode<int> x = new(graph, 8);
SourceNode<int> y = new(graph, 9);
DoubleNode<int> xPlusY = new(graph, x, y);

visualiser.Outputs.Subscribe(Console.WriteLine);

...
    
graph.Prime(); // x = 8, y = 9, xPlusY = 17

y.Fire(0); // y = 0, xPlusY = 8
```

The subscription will be called on each update to the descriptors and dependent on the descriptor type will
provide all descriptors each time there is a change, or just the changed descriptors (as above).

## Node IDs

Nodes are not assigned an ID by the graph originally, so to help with correlating node information from the various
descriptors the visualiser assigns its own IDs to nodes.

Because this is done by the visualiser, this means that there is **no guarantee** that IDs produced from different
visualiser instances will match.