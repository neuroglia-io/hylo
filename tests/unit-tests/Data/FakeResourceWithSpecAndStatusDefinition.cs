using Json.Schema;
using System.Runtime.Serialization;

namespace Hylo.UnitTests.Data;

[DataContract]
internal class FakeResourceWithSpecAndStatusDefinition
    : ResourceDefinition
{

    internal new const string ResourceGroup = "unit.tests.hylo.io";
    internal new const string ResourceVersion = "v1";
    internal new const string ResourceSingular = "fake";
    internal new const string ResourcePlural = "fakes";
    internal new const string ResourceKind = "fake";

    public FakeResourceWithSpecAndStatusDefinition()
        : base(new(ResourceScope.Namespaced, ResourceGroup, new(ResourceSingular, ResourcePlural, ResourceKind), new ResourceDefinitionVersion(ResourceVersion, new(GetSchema())) { Served = true, Storage = true }))
    {

    }

    static JsonSchema GetSchema()
    {
        return JsonSchema.FromText($$"""
{
  "type": "object",
  "properties": {
    "spec":{
      "type": "object",
      "properties":{
        "fakeProperty1":{
          "type": "string"
        },
        "fakeProperty2":{
          "type": "number"
        }
      }
    },
    "status":{
      "type": "object",
      "properties":{
        "fakeProperty1":{
          "type": "string"
        },
        "fakeProperty2":{
          "type": "number"
        }
      }
    }
  }
}
""");
    }

}
