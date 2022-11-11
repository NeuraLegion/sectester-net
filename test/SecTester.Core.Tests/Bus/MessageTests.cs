using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.Bus;

public class MessageTests
{
  [Fact]
  public void Message_OnlyPayload_SetDefaultValuesToProps()
  {
    // arrange
    const string payload = "text";
    var expected = new
    {
      Payload = payload,
      Type = "TestMessage",
      CreatedAt = DateTime.Now,
      CorrelationId = Guid.Empty.ToString()
    };

    // act
    var message = new TestMessage(payload);

    // assert
    message.Should().BeEquivalentTo(expected, options => options
      .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds())).WhenTypeIs<DateTime>()
      .Using<string>(ctx => ctx.Subject.Should().BeOfType<string>()).When(info => info.Path.EndsWith("CorrelationId")));
  }

  [Fact]
  public void Message_GivenPayload_OverrideDefaultValues()
  {
    // arrange
    const string payload = "text";
    const string type = "SomeMessage";
    const string correlationId = "random";
    DateTime createdAt = DateTime.Now;

    var expected = new
    {
      Payload = payload,
      Type = type,
      CreatedAt = createdAt,
      CorrelationId = correlationId
    };

    // act
    var message = new TestMessage(payload, type, correlationId, createdAt);

    // assert
    message.Should().BeEquivalentTo(expected);
  }
}
