using SecTester.Core.Bus;

namespace SecTester.Core.Tests.Bus;

public class CommandTests
{
  [Fact]
  public void Command_OnlyPayload_SetDefaultValuesToProps()
  {
    // arrange
    const string payload = "text";
    var expected = new
    {
      Payload = payload,
      Type = "TestCommand",
      CreatedAt = DateTime.Now,
      CorrelationId = Guid.Empty.ToString(),
      ExpectReply = true,
      Ttl = 10000
    };

    // act
    var message = new TestCommand(payload);

    // assert
    message.Should().BeEquivalentTo(expected, options => options
      .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds())).WhenTypeIs<DateTime>()
      .Using<string>(ctx => ctx.Subject.Should().BeOfType<string>()).When(info => info.Path.EndsWith("CorrelationId")));
  }

  [Fact]
  public void Command_TtlIsGreaterThan0_SetTtl()
  {
    // arrange
    const string payload = "text";
    const int ttl = 1;
    var expected = new
    {
      Ttl = ttl
    };

    // act
    var message = new TestCommand(payload, ttl: ttl);

    // assert
    message.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Command_TtlIsLessThan0_IgnoreOption()
  {
    // arrange
    const string payload = "text";
    const int ttl = -1;
    var expected = new
    {
      Ttl = 10000
    };

    // act
    var message = new TestCommand(payload, ttl: ttl);

    // assert
    message.Should().BeEquivalentTo(expected);
  }


  private class TestCommand : Command<string, string?>
  {
    public TestCommand(string payload, string? type = null, string? correlationId = null, DateTime? createdAt = null, bool? expectReply = null, int? ttl = null) : base(payload, type, correlationId, createdAt, expectReply, ttl)
    {
    }
  }
}
