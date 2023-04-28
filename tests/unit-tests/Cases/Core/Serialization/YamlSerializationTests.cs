using AutoBogus;

namespace Hylo.UnitTests.Cases.Core.Serialization;

public class YamlSerializationTests
{

    [Fact]
    public void SerializeDeserialize_AllResources_Should_Work()
    {
        //arrange
        var resourceTypes = typeof(Resource).Assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsGenericType && t.GetConstructor(Array.Empty<Type>()) != null && t != typeof(Resource) && typeof(Resource).IsAssignableFrom(t));
        var faker = AutoFaker.Create();

        var toRemove = DateTimeOffset.Now;
        var ser = Serializer.Yaml.Serialize(toRemove);
        var des = Serializer.Yaml.Deserialize<DateTimeOffset>(ser);

        //act
        foreach (var resourceType in resourceTypes)
        {
            var serializedResource = faker.Generate(resourceType);
            var yaml = Serializer.Yaml.Serialize(serializedResource, resourceType);
            var deserializedResource = Serializer.Yaml.Deserialize(yaml, resourceType);
            try
            {
                deserializedResource.Should().BeEquivalentTo(serializedResource);
            }
            catch(Exception ex)
            {
                var yaml1 = Serializer.Yaml.Serialize(serializedResource, resourceType);
                throw;
            }
        }
    }

}