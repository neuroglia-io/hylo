namespace Hylo.Api.Configuration;

/// <summary>
/// Represents the default implementation of the <see cref="IHyloApiResourceInitializerOptionsBuilder"/> interface
/// </summary>
public class HyloApiResourceInitializerOptionsBuilder
    : IHyloApiResourceInitializerOptionsBuilder
{

    /// <summary>
    /// Gets the <see cref="HyloApiResourceInitializerOptions"/> to configure
    /// </summary>
    protected HyloApiResourceInitializerOptions Options { get; } = new();
   
    /// <inheritdoc/>
    public virtual IHyloApiResourceInitializerOptionsBuilder RegisterResourceDefinition(V1ResourceDefinition resourceDefinition) 
    {
        if (this.Options.ResourceDefinitions == null) this.Options.ResourceDefinitions = new();
        this.Options.ResourceDefinitions.Add(resourceDefinition);
        return this;
    }

    /// <inheritdoc/>
    public virtual HyloApiResourceInitializerOptions Build() => this.Options;

}