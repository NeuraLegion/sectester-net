using SecTester.Bus.Dispatchers;

namespace SecTester.Repeater.Tests.Bus;

public class DefaultRepeaterEventBusFactoryTests : IDisposable
{
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";
  private const string Hostname = "app.neuralegion.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";

  private readonly RmqEventBusFactory _eventBusFactory = Substitute.For<RmqEventBusFactory>();

  public void Dispose()
  {
    _eventBusFactory.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Create_CreatesEventBus()
  {
    // arrange
    Configuration config = new(Hostname, new Credentials(Token));
    DefaultRepeaterEventBusFactory sut = new(config, _eventBusFactory);

    // act
    using var eventBus = sut.Create(Id);

    // assert
    _eventBusFactory.Received()
      .CreateEventBus(Arg.Is<RmqEventBusOptions>(x =>
        x.Username == "bot" &&
        x.Password == config.Credentials!.Token &&
        x.Url == config.Bus &&
        x.ClientQueue.Contains(Id) &&
        x.Exchange == "EventBus" &&
        x.AppQueue == "app"));
  }

  [Fact]
  public void Create_CredentialsNotDefined_ThrowsError()
  {
    // arrange
    Configuration config = new(Hostname);
    DefaultRepeaterEventBusFactory sut = new(config, _eventBusFactory);

    // act
    var act = () => sut.Create(Id);

    // assert
    act.Should().Throw<InvalidOperationException>();
  }
}
