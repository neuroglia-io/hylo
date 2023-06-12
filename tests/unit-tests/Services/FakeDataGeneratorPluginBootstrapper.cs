using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Services;

public class FakeDataGeneratorPluginBootstrapper
    : IPluginBootstrapper
{

    public const string InjectedString = "Plugin Bootstrapper Works";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(provider => InjectedString);
    }

}
