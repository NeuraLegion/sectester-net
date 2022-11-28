namespace SecTester.Repeater.Tests.Api;

public class DefaultRepeatersTests : IDisposable
{
  private readonly CommandDispatcher _commandDispatcher;
  private readonly DefaultRepeaters _sut;

  public DefaultRepeatersTests()
  {
    _commandDispatcher = Substitute.For<CommandDispatcher>();
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
    _commandDispatcher.Execute(Arg.Any<ListRepeatersRequest>()).Returns(new List<RepeaterIdentity>
    {
      new("142", "bar"), new("42", "foo")
    });

    // act
    var result = await _sut.CreateRepeater("foo");

    //
    result.Should().Be("42");
  }

  [Fact]
  public async Task CreateRepeater_CreatedRepeaterNotFound_ThrowsError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<ListRepeatersRequest>()).Returns(new List<RepeaterIdentity>
    {
      new("142", "bar")
    });

    // act
    var act = () => _sut.CreateRepeater("foo");

    //
    await act.Should().ThrowAsync<SecTesterException>()
      .WithMessage("Cannot find created repeater id");
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
