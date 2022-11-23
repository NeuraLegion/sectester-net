using SecTester.Core.Extensions;

namespace SecTester.Bus.Tests.Extensions;

public class MessageSerializerExtensionsTests
{
  private readonly ServiceCollection _services = new();

  public MessageSerializerExtensionsTests()
  {
    var config = new Configuration("app.neuralegion.com",
      new Credentials("0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk"));
    _services.AddSecTesterConfig(config);
    _services.AddSecTesterBus();
  }

  [Fact]
  public void SerializeJsonContent_ReturnsContentWithJsonMediaType()
  {
    
    // arrange
    var messageSerializer = _services.BuildServiceProvider().GetRequiredService<MessageSerializer>();

    // act
    var content = messageSerializer.SerializeJsonContent(new { foo = 1 });
    
    // assert
    content.Headers.ContentType.Should().NotBeNull();
    content.Headers.ContentType.MediaType.Should().Be("application/json");
    content.Headers.ContentType.CharSet.Should().Be("utf-8");
  }
}
