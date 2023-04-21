namespace Hylo.Infrastructure;

/// <summary>
/// Defines extensions for <see cref="IRepositoryOptionsBuilder"/>s
/// </summary>
public static class IRepositoryOptionsBuilderExtensions
{

    /// <summary>
    /// Seeds the <see cref="IRepository"/> to configure with the specified <see cref="IResourceDefinition"/>
    /// </summary>
    /// <param name="builder">The <see cref="IRepositoryOptionsBuilder"/> to configure</param>
    /// <returns>The configured <see cref="IRepositoryOptionsBuilder"/></returns>
    public static IRepositoryOptionsBuilder WithDefinition<TDefinition>(this IRepositoryOptionsBuilder builder)
        where TDefinition : class, IResourceDefinition, new()
    {
        return builder.WithDefinition(new TDefinition());
    }

}