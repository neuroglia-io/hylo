using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.UnitTests.Services;

[Plugin(typeof(IFakeDataGenerator), typeof(FakeDataGeneratorPluginBootstrapper))]
public class FakeDataGeneratorPlugin
    : IFakeDataGenerator
{

    public FakeDataGeneratorPlugin(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
    }

    protected IServiceProvider ServiceProvider { get; }

    public object GenerateFakeData() => this.ServiceProvider.GetRequiredService<string>();

}
