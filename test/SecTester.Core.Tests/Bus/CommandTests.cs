namespace SecTester.Core.Tests.Bus;

public class CommandTests : IDisposable
{
  private readonly CommandDispatcher _dispatcher;

  public CommandTests()
  {
    _dispatcher = Substitute.For<CommandDispatcher>();
  }

  public void Dispose()
  {
    _dispatcher.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

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
    var message = new TestCommandWithTtl(payload, ttl);

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
    var message = new TestCommandWithTtl(payload, ttl);

    // assert
    message.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Command_Executes()
  {
    // arrange
    const string payload = "text";
    var command = new TestCommand(Payload: payload);
    _dispatcher.Execute(command).Returns(Task.FromResult<string?>(null));

    // act
    command.Execute(_dispatcher);

    // assert
    _dispatcher.Received(1).Execute(command);
  }

  [Fact]
  public async Task Command_ExecutesWithResult()
  {
    // arrange
    const string payload = "text";
    const string expected = "result";
    var command = new TestCommand(Payload: payload);
    _dispatcher.Execute(command)!.Returns(Task.FromResult(expected));

    // act
    var result = await command.Execute(_dispatcher);

    // assert
    result.Should().Be(expected);
  }

  [Fact]
  public void Command_WhenException_RethrowsError()
  {
    // arrange
    const string payload = "text";
    var command = new TestCommand(Payload: payload);
    _dispatcher.Execute(command).ThrowsAsync<Exception>();

    // act
    var act = () => command.Execute(_dispatcher);

    // assert
    act.Should().ThrowAsync<Exception>();
  }

  private record TestCommand(string Payload) : Command<string?>
  {
    public string Payload = Payload;
  }

  private record TestCommandWithTtl : TestCommand
  {
    public TestCommandWithTtl(string payload, int ttl) : base(payload)
    {
      Ttl = ttl;
    }
  }
}
