namespace SecTester.Bus.Tests.Dispatchers;

public class RmqEventBusOptionsTests
{
  private const string Url = "amqp://example.com";
  private const string AppQueue = "app";
  private const string Exchange = "bus";
  private const string ClientQueue = "client";

  [Fact]
  public void RmqEventBusOptions_SetsDefaultOptions()
  {
    // arrange
    var timeout = TimeSpan.FromSeconds(30);

    // act
    var result = new RmqEventBusOptions(Url, AppQueue, Exchange, ClientQueue);

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url,
      Exchange,
      AppQueue,
      ClientQueue,
      PrefetchCount = 1,
      ReconnectTime = timeout,
      HeartbeatInterval = timeout,
      ConnectTimeout = timeout
    });
  }

  [Fact]
  public void RmqEventBusOptions_OverrideDefaultOptions()
  {
    // arrange
    var timeout = TimeSpan.FromSeconds(30);
    var newTimeout = TimeSpan.FromSeconds(20);
    const int newPrefetchCount = 30;

    // act
    var result = new RmqEventBusOptions(Url, AppQueue, Exchange, ClientQueue)
    {
      HeartbeatInterval = newTimeout,
      PrefetchCount = newPrefetchCount
    };

    // assert
    result.Should().BeEquivalentTo(new
    {
      Url,
      Exchange,
      AppQueue,
      ClientQueue,
      PrefetchCount = newPrefetchCount,
      ReconnectTime = timeout,
      HeartbeatInterval = newTimeout,
      ConnectTimeout = timeout
    });
  }
}
