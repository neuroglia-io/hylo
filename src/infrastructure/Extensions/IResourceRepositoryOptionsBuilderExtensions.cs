namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IResourceRepositoryOptionsBuilder"/>s
/// </summary>
public static class IResourceRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Seeds the <see cref="IResourceRepository"/> to configure with the specified <see cref="IResourceDefinition"/>
    /// </summary>
    /// <param name="builder">The <see cref="IResourceRepositoryOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IResourceRepositoryOptionsBuilder"/></returns>
    public static IResourceRepositoryOptionsBuilder WithDefinition<TDefinition>(this IResourceRepositoryOptionsBuilder builder)
        where TDefinition : class, IResourceDefinition, new()
    {
        return builder.WithDefinition(new TDefinition());
    }

}