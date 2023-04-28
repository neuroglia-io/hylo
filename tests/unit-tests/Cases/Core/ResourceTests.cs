using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hylo.UnitTests.Cases.Core;

public class ResourceTests
{

    [Fact]
    public void Resource_WithNoMetadata_ShouldFail()
    {
        //act
        var test = () => new Resource(FakeResourceWithSpecAndStatus.ResourceDefinition, null!);

        //assert
        test.Should().Throw<ArgumentException>();
    }

    [Theory, MemberData(nameof(InvalidNames))]
    public void Resource_WithInvalidName_ShouldFail(string name)
    {
        //act
        var test = () => new Resource(FakeResourceWithSpecAndStatus.ResourceDefinition, new(name));

        //assert
        test.Should().Throw<ArgumentException>();
    }

    [Theory, MemberData(nameof(InvalidNamespaces))]
    public void Resource_WithInvalidNamespace_ShouldFail(string @namespace)
    {
        //act
        var test = () => new Resource(FakeResourceWithSpecAndStatus.ResourceDefinition, new("fake-name", @namespace));

        //assert
        test.Should().Throw<ArgumentException>();
    }

    [Theory, MemberData(nameof(InvalidLabels))]
    public void Resource_WithInvalidLabels_ShouldFail(string labelName)
    {
        //act
        var test = () => new Resource(FakeResourceWithSpecAndStatus.ResourceDefinition, new("fake-name", null, new Dictionary<string, string>() { { labelName, "fake-label-value" } }));

        //assert
        test.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Resource_DistinctUntilChanged_ShouldWork()
    {
        //arrange
        var resource = FakeResourceWithSpecAndStatus.Create("fake-namespace");
        var resourceClone = resource.Clone()!;

        var observable = new BehaviorSubject<FakeResourceWithSpecAndStatus>(resource);
        var changeCount = 0;
        observable.DistinctUntilChanged().Skip(1).Subscribe(r => changeCount++);

        //act
        observable.OnNext(resource);
        observable.OnNext(resourceClone);
        observable.OnNext(resource with { Metadata = resource.Metadata with { Name = "updated-fake" } });

        //assert
        changeCount.Should().Be(1);

    }

    public static IEnumerable<object[]> InvalidNames 
    {
        get
        {
            yield return new [] { (string)null! }!;
            yield return new[] { "" }!;
            yield return new[] { " " }!;
            yield return new[] { "PascalCased" }!;
            yield return new[] { "-lowercased" }!;
            yield return new[] { "lowercased-" }!;
            yield return new[] { "lower_cased" }!;
            yield return new[] { "lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long" }!;
        }
    }

    public static IEnumerable<object[]> InvalidNamespaces
    {
        get
        {
            yield return new[] { " " }!;
            yield return new[] { "PascalCased" }!;
            yield return new[] { "-lowercased" }!;
            yield return new[] { "lowercased-" }!;
            yield return new[] { "lower_cased" }!;
            yield return new[] { "lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long" }!;
        }
    }

    public static IEnumerable<object[]> InvalidLabels
    {
        get
        {
            yield return new[] { (string)null! }!;
            yield return new[] { "" }!;
            yield return new[] { " " }!;
            yield return new[] { "PascalCased" }!;
            yield return new[] { "-lowercased" }!;
            yield return new[] { "lowercased-" }!;
            yield return new[] { "lower_cased" }!;
            yield return new[] { "lower/cased/value" }!;
            yield return new[] { "lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long-lowercased-but-far-too-long" }!;
        }
    }

}
