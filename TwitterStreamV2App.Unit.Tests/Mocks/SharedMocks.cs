using Microsoft.Extensions.Options;
using Moq;

namespace TwitterStreamV2App.Unit.Tests.Mocks;

public class SharedMocks
{
    public static IOptionsSnapshot<T> CreateIOptionSnapshotMock<T>(T value) where T : class, new()
    {
        var mock = new Mock<IOptionsSnapshot<T>>();
        mock.Setup(m => m.Value).Returns(value);
        return mock.Object;
    }
}