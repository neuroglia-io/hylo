namespace Hylo.UnitTests.Cases.Core;

public class EquatableListTests
{

    [Fact]
    public void EquatableLists_WithIdenticalItems_Should_Match()
    {
        //arrange
        var @namespace = "fake-namespace";
        var timestamp = DateTimeOffset.UtcNow;

        //act
        var list1 = new EquatableList<FakeResourceWithSpecAndStatus>();
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < 10; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource.Metadata.CreationTimestamp = timestamp;
            list1.Add(resource);
        }
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        var list2 = new EquatableList<FakeResourceWithSpecAndStatus>();
        for (int i = 0; i < 10; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource.Metadata.CreationTimestamp = timestamp;
            list2.Add(resource);
        }

        //assert
        list1.Should().Equal(list2);
        list1.As<object>().Should().Be(list2);
    }

    [Fact]
    public void EquatableLists_WithDifferentItems_Should_NotMatch()
    {
        //arrange
        var @namespace = "fake-namespace";
        var timestamp = DateTimeOffset.UtcNow;

        //act
        var list1 = new EquatableList<FakeResourceWithSpecAndStatus>();
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        for (int i = 0; i < 10; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource.Metadata.CreationTimestamp = timestamp;
            list1.Add(resource);
        }
        FakeResourceWithSpecAndStatus.AutoIncrementIndex = 0;
        var list2 = new EquatableList<FakeResourceWithSpecAndStatus>();
        for (int i = 0; i < 10; i++)
        {
            var resource = FakeResourceWithSpecAndStatus.Create(@namespace);
            resource.Metadata.CreationTimestamp = timestamp;
            list2.Add(resource);
        }
        list2.Last().Metadata.CreationTimestamp = DateTimeOffset.UtcNow;

        //assert
        list1.Should().NotEqual(list2);
        list1.As<object>().Should().NotBe(list2);
    }

}