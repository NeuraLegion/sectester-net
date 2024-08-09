namespace SecTester.Repeater.Tests.Bus;

public class DefaultRepeaterBusFactoryTests : IDisposable
{
  private const string Id = "g5MvgM74sweGcK1U6hvs76";
  private const string Hostname = "app.brightsec.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";

  private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();

  public void Dispose()
  {
    _loggerFactory.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Create_CreatesBus()
  {
    // arrange
    Configuration config = new(Hostname, new Credentials(Token));
    DefaultRepeaterBusFactory sut = new(config, _loggerFactory);

    // act
    await using var bus = sut.Create(Id);

    // assert
    bus.Should().BeAssignableTo<IRepeaterBus>();
  }

  [Fact]
  public async Task Create_CredentialsNotDefined_ThrowsError()
  {
    // arrange
    Configuration config = new(Hostname);
    DefaultRepeaterBusFactory sut = new(config, _loggerFactory);

    // act
    var act = async () =>
    {
      await using var _ = sut.Create(Id);
    };

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>();
  }
}
