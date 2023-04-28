using System.Runtime.Serialization;

namespace Hylo.UnitTests.Data;

[DataContract]
internal record FakeResourceWithSpecAndStatus
    : Resource<FakeResourceSpec, FakeResourceStatus>
{

    public static uint AutoIncrementIndex { get; set; }

    internal static readonly ResourceDefinitionInfo ResourceDefinition = new FakeResourceWithSpecAndStatusDefinition()!;

    public FakeResourceWithSpecAndStatus() : base(ResourceDefinition) { }

    public FakeResourceWithSpecAndStatus(ResourceMetadata metadata, FakeResourceSpec spec, FakeResourceStatus? status = null) 
        : base(ResourceDefinition, metadata, spec, status)
    {

    }

    public static FakeResourceWithSpecAndStatus Create(string @namespace, IDictionary<string, string>? labels = null)
    {
        AutoIncrementIndex++;
        return new FakeResourceWithSpecAndStatus(new($"fake-resource-{AutoIncrementIndex}", @namespace, labels), new(), new());
    }

}
