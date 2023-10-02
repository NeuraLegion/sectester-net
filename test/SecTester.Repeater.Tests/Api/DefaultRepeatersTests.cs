namespace SecTester.Repeater.Tests.Api;

public class DefaultRepeatersTests : IDisposable
{
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";

  private readonly ICommandDispatcher _commandDispatcher;
  private readonly DefaultRepeaters _sut;

  public DefaultRepeatersTests()
  {
    _commandDispatcher = Substitute.For<ICommandDispatcher>();
    _sut = new DefaultRepeaters(_commandDispatcher);
  }

  public void Dispose()
  {
    _commandDispatcher.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task CreateRepeater_CreatesRepeater()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateRepeaterRequest>()).Returns(new RepeaterIdentity(Id, "foo"));

    // act
    var result = await _sut.CreateRepeater("foo");

    // assert
    result.Should().Be(Id);
  }

  [Fact]
  public async Task CreateRepeater_ThrowsError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<CreateRepeaterRequest>()).Returns(new RepeaterIdentity("", "foo"));

    // act
    var act = () => _sut.CreateRepeater("foo");

    // assert
    await act.Should().ThrowAsync<SecTesterException>()
      .WithMessage("Cannot create repeater");
  }

  [Fact]
  public async Task DeleteRepeater_RemovesRepeater()
  {
    // act
    await _sut.DeleteRepeater("foo");

    // assert
    await _commandDispatcher.Received(1).Execute(Arg.Any<DeleteRepeaterRequest>());
  }
}
