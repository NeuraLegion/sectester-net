namespace SecTester.Repeater.Tests;

public class RepeaterTests : IDisposable, IAsyncDisposable
{
  private const string Version = "42.0.1";
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";

  private readonly IRepeaterBus _bus = Substitute.For<IRepeaterBus>();
  private readonly MockLogger<Repeater> _logger = Substitute.For<MockLogger<Repeater>>();
  private readonly IAnsiCodeColorizer _ansiCodeColorizer = Substitute.For<IAnsiCodeColorizer>();
  private readonly RequestRunnerResolver _resolver = Substitute.For<RequestRunnerResolver>();

  private readonly Repeater _sut;

  public RepeaterTests()
  {
    var version = new Version(Version);
    _sut = new Repeater(_bus, version, _logger, _ansiCodeColorizer, _resolver);
  }

  public async ValueTask DisposeAsync()
  {
    Dispose();
    await _sut.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void Dispose()
  {
    _ansiCodeColorizer.ClearSubstitute();
    _logger.ClearSubstitute();
    _bus.ClearSubstitute();
    _resolver.ClearSubstitute();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Start_DeploysItself()
  {
    // arrange
    _bus.Deploy(null, Arg.Any<CancellationToken>()).Returns(Task.FromResult<string>(Id));

    // act
    await _sut.Start();

    // assert
    _sut.RepeaterId.Should().Be(Id);
  }

  [Fact]
  public async Task Start_OperationCancelled_ReThrowsError()
  {
    // arrange
    using var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.Cancel();

    // act
    var act = () => _sut.Start(cancellationTokenSource.Token);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Fact]
  public async Task Start_SetsStatusToRunningJustAfterCall()
  {
    // act
    await _sut.Start();

    // assert
    _sut.Status.Should().Be(RunningStatus.Running);
  }

  [Fact]
  public async Task Start_RepeaterIsStarting_ThrowsError()
  {
    // arrange
    await _sut.Start();

    // act
    var act = () => _sut.Start();

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repeater is already active.");
  }

  [Fact]
  public async Task Start_NewVersionIsAvailable_ShowsWarning()
  {
    // arrange
    var newVersion = new Regex(@"(\d+)").Replace(Version, x => $"{int.Parse(x.Groups[0].Value) + 1}");
    await _sut.Start();

    // act
    _bus.UpgradeAvailable += Raise.Event<Action<Version>>(new Version(newVersion));

    // assert
    _logger.Received().Log(LogLevel.Warning, Arg.Is<string>(s => s.Contains($"A new Repeater version ({newVersion}) is available")));
  }

  [Fact]
  public async Task Stop_RunningRepeater_Stops()
  {
    // arrange
    await _sut.Start();

    // act
    await _sut.Stop();

    // assert
    await _bus.Received().DisposeAsync();
  }

  [Fact]
  public async Task Stop_RunningRepeater_EntersIntoOff()
  {
    // arrange
    await _sut.Start();

    // act
    await _sut.Stop();

    // assert
    _sut.Status.Should().Be(RunningStatus.Off);
  }

  [Fact]
  public async Task Stop_RepeaterIsOff_DoesNothing()
  {
    // act
    await _sut.Stop();

    // assert
    _sut.Status.Should().Be(RunningStatus.Off);
  }

  [Fact]
  public async Task Stop_RepeaterIsOff_IgnoresSecondCall()
  {
    // arrange
    await _sut.Start();
    await _sut.Stop();

    // act
    await _sut.Stop();

    // assert
    _sut.Status.Should().Be(RunningStatus.Off);
  }

  [Fact]
  public async Task Stop_OperationCancelled_ThrowsError()
  {
    // arrange
    using var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.Cancel();

    // act
    var act = () => _sut.Stop(cancellationTokenSource.Token);

    // assert
    await act.Should().ThrowAsync<OperationCanceledException>();
  }

  [Fact]
  public async Task DisposeAsync_StopsRepeater()
  {
    // arrange
    await _sut.Start();

    // act
    await _sut.DisposeAsync();

    // assert
    await _bus.Received().DisposeAsync();
  }

  [Fact]
  public async Task Start_IncomingRequest_ExecutesRequest()
  {
    // arrange
    var request = new IncomingRequest(new Uri("http://foo.bar"));
    var response = new OutgoingResponse
    {
      StatusCode = 200,
    };
    _resolver(Protocol.Http)!.Run(request).Returns(response);
    await _sut.Start();

    // act
    _bus.RequestReceived += Raise.Event<Func<IncomingRequest, Task<OutgoingResponse>>>(request);

    // assert
    await _resolver(Protocol.Http)!.Received().Run(Arg.Any<IRequest>());
  }

  [Fact]
  public async Task Start_IncomingRequest_LogsError()
  {
    // arrange
    var request = new IncomingRequest(new Uri("http://foo.bar"));
    _resolver(Arg.Any<Protocol>()).ReturnsNull();
    await _sut.Start();

    // act
    _bus.RequestReceived += Raise.Event<Func<IncomingRequest, Task<OutgoingResponse>>>(request);

    // assert
    _logger.Received().Log(LogLevel.Error, Arg.Is<string>(s => s.Contains($"Unsupported protocol {request.Protocol}")));
  }

  [Fact]
  public async Task Start_ErrorReceived_LogsError()
  {
    // arrange
    var error = new Exception("Something went wrong");
    await _sut.Start();

    // act
    _bus.ErrorOccurred += Raise.Event<Action<Exception>>(error);

    // assert
    _logger.Received().Log(LogLevel.Error, Arg.Is<string>(s => s.Contains($": {error.Message}")));
  }
}
