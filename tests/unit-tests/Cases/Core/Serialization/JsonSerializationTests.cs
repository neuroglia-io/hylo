using AutoBogus;

namespace Hylo.UnitTests.Cases.Core.Serialization;

public class JsonSerializationTests
{

    [Fact]
    public void SerializeDeserialize_AllResources_Should_Work()
    {
        //arrange
        var resourceTypes = typeof(Resource).Assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsGenericType && t.GetConstructor(Array.Empty<Type>()) != null && t != typeof(Resource) && typeof(Resource).IsAssignableFrom(t));
        var faker = AutoFaker.Create();

        //act
        foreach (var resourceType in resourceTypes)
        {
            var serializedResource = faker.Generate(resourceType);
            var json = Serializer.Json.Serialize(serializedResource, resourceType);
            var deserializedResource = Serializer.Json.Deserialize(json, resourceType);
            try
            {
                deserializedResource.Should().BeEquivalentTo(serializedResource);
            }
            catch (Exception ex)
            {
                var json1 = Serializer.Json.Serialize(serializedResource, resourceType);
                throw;
            }

        }
    }

}
