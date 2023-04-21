using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.UnitTests;

internal static class IResourceRepositoryBuilderExtensions
{

    internal static ServiceProvider? ServiceProvider;

    internal static async ValueTask<IRepository> BuildAsync(this IRepositoryOptionsBuilder builder, CancellationToken cancellationToken = default)
    {
        builder.Build();
        ServiceProvider = builder.Services.BuildServiceProvider();
        foreach(var hostedService in ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(cancellationToken).ConfigureAwait(false);
        }
        return ServiceProvider.GetRequiredService<IRepository>();
    }

}
