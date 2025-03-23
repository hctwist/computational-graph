using ComputationalGraph.Core;

namespace ComputationalGraph.Visualisation.Descriptors.Containers;

/// <summary>
/// Allows subscription and retrieval of descriptors.
/// </summary>
/// <typeparam name="TDescriptor">The descriptor type.</typeparam>
public abstract class DescriptorContainer<TDescriptor>
{
    /// <summary>
    /// Called when an update to the descriptor occurs.
    /// </summary>
    public delegate void OnUpdate(TDescriptor descriptor);
    
    /// <summary>
    /// The graph.
    /// </summary>
    protected readonly Graph Graph;
    
    /// <summary>
    /// The node ID designator.
    /// </summary>
    protected readonly NodeIdDesignator NodeIdDesignator;

    /// <summary>
    /// The registered subscriptions.
    /// </summary>
    private readonly List<OnUpdate> subscriptions;
    
    /// <summary>
    /// Creates a new <see cref="DescriptorContainer{TDescriptor}"/>.
    /// </summary>
    /// <param name="graph">The graph.</param>
    /// <param name="nodeIdDesignator">The node ID designator.</param>
    protected DescriptorContainer(Graph graph, NodeIdDesignator nodeIdDesignator)
    {
        Graph = graph;
        NodeIdDesignator = nodeIdDesignator;

        subscriptions = [];
    }

    /// <summary>
    /// Gets the current value of the descriptor.
    /// </summary>
    /// <returns>The descriptor.</returns>
    public abstract TDescriptor Get();

    /// <summary>
    /// Subscribes to updates of this descriptor. If necessary an initial update will be published as soon as this is called.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    public void Subscribe(OnUpdate subscription)
    {
        subscriptions.Add(subscription);
        
        if (Graph.State is not GraphState.Building)
        {
            subscription(Get());
        }
    }

    /// <summary>
    /// Notifies subscribers of descriptor updates.
    /// </summary>
    /// <param name="getDescriptorUpdate">A function to get the descriptor update.</param>
    protected void NotifySubscribers(Func<TDescriptor> getDescriptorUpdate)
    {
        if (subscriptions.Count == 0)
        {
            return;
        }

        TDescriptor descriptor = getDescriptorUpdate();
        
        foreach (OnUpdate subscription in subscriptions)
        {
            subscription(descriptor);
        }
    }
}