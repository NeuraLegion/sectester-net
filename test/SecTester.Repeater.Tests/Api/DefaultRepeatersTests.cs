namespace SecTester.Repeater.Tests.Api;

public class DefaultRepeatersTests : IDisposable
{
  const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";
  const string AnotherId = "220baaac-b7ec-46a7-ab5e-ff1e96b0785e";

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
    _commandDispatcher.Execute(Arg.Any<ListRepeatersRequest>()).Returns(new List<RepeaterIdentity>
    {
      new(AnotherId, "bar"), new(Id, "foo")
    });

    // act
    var result = await _sut.CreateRepeater("foo");

    //
    result.Should().Be(Id);
  }

  [Fact]
  public async Task CreateRepeater_CreatedRepeaterNotFound_ThrowsError()
  {
    // arrange
    _commandDispatcher.Execute(Arg.Any<ListRepeatersRequest>()).Returns(new List<RepeaterIdentity>
    {
      new(AnotherId, "bar")
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
