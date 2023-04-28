namespace Hylo.UnitTests.Cases.Core;

public class EmbeddedResourcesTests
{

    [Fact]
    public void ReadAndDeserialize_EmbeddedResourceAssets_ShouldWork()
    {
        //act
        var resourceDefinition = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.ResourceDefinition));
        var mutatingWebhook = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.MutatingWebhook));
        var validatingWebhook = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.ValidatingWebhook));

        //assert
        resourceDefinition.Should().NotBeNull();
        mutatingWebhook.Should().NotBeNull();
        validatingWebhook.Should().NotBeNull();
    }

}
