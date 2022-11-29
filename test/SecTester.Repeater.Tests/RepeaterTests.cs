using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.Extensions.Logging;
using SecTester.Core.Utils;
using SecTester.Repeater.Bus;
using SecTester.Repeater.Tests.Mocks;

namespace SecTester.Repeater.Tests;

public class RepeaterTests : IDisposable
{
  private const string Version = "42.0.1";
  private const string Id = "99138d92-69db-44cb-952a-1cd9ec031e20";

  private readonly EventBus _eventBus;
  private readonly MockLogger _logger;
  private readonly TimerProvider _timerProvider;
  private readonly Repeater _sut;

  public RepeaterTests()
  {
    _eventBus = Substitute.For<EventBus>();
    _logger = Substitute.For<MockLogger>();
    _timerProvider = Substitute.For<TimerProvider>();
    var version = new Version(Version);
    _sut = new Repeater(Id, _eventBus, version, _logger, _timerProvider);
  }

  public void Dispose()
  {
    _logger.ClearSubstitute();
    _eventBus.ClearSubstitute();
    _timerProvider.ClearSubstitute();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Start_RegistersInApp()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));

    // act
    await _sut.Start();

    // assert
    await _eventBus.Received().Execute(Arg.Any<RegisterRepeaterCommand>());
    await _eventBus.Received().Publish(Arg.Is<RepeaterStatusEvent>(x => x.Status == RepeaterStatus.Connected && x.RepeaterId == Id));
  }

  [Fact]
  public async Task Start_RegistrationFailed_RegistersInApp()
  {
    // act
    var act = () => _sut.Start();

    // assert
    await act.Should().ThrowAsync<SecTesterException>().WithMessage("Error registering repeater.");
  }

  [Fact]
  public async Task Start_SendsPingPeriodically()
  {
    // arrange
    var elapsedEventArgs = EventArgs.Empty as ElapsedEventArgs;
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));

    // act
    await _sut.Start();

    // assert
    _timerProvider.Elapsed += Raise.Event<ElapsedEventHandler>(new object(), elapsedEventArgs);
    await _eventBus.Received(2).Publish(Arg.Is<RepeaterStatusEvent>(x => x.Status == RepeaterStatus.Connected && x.RepeaterId == Id));
  }

  [Fact]
  public async Task Start_SetsStatusToRunningJustAfterCall()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));

    // act
    await _sut.Start();

    // assert
    _sut.Status.Should().Be(RunningStatus.Running);
  }

  [Fact]
  public async Task Start_RepeaterIsStarting_ThrowsError()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
    await _sut.Start();

    // act
    var act = () => _sut.Start();

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Repeater is already active.");
  }

  [Fact]
  public async Task Start_AbortedByError_Restarts()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(null, new RegisterRepeaterResult(Version));

    // act
    var act = () => _sut.Start();

    // assert
    await act.Should().ThrowAsync<Exception>();
    await act.Should().NotThrowAsync<Exception>();
  }

  public static readonly IEnumerable<object[]> RegistrationErrors = new List<object[]>
  {
    new object[]
    {
      new RegisterRepeaterResult(Error: RepeaterRegisteringError.Busy), $"There is an already running Repeater with ID {Id}"
    },
    new object[]
    {
      new RegisterRepeaterResult(Error: RepeaterRegisteringError.NotActive), "The current Repeater is not active"
    },
    new object[]
    {
      new RegisterRepeaterResult(Error: RepeaterRegisteringError.NotFound), "Unauthorized access"
    },
    new object[]
    {
      new RegisterRepeaterResult(Error: RepeaterRegisteringError.RequiresToBeUpdated),
      "The current running version is no longer supported"
    },
    new object[]
    {
      new RegisterRepeaterResult(Error: (RepeaterRegisteringError)(-100)),
      "Something went wrong. Unknown error."
    }
  };

  [Theory]
  [MemberData(nameof(RegistrationErrors))]
  public async Task Start_ErrorWhileRegistration_ThrowsError(RegisterRepeaterResult input, string expected)
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(input);

    // act
    var act = () => _sut.Start();

    // assert
    await act.Should().ThrowAsync<Exception>().WithMessage($"*{expected}*");
  }

  [Fact]
  public async Task Start_NewVersionIsAvailable_ShowsWarning()
  {
    // arrange
    var newVersion = new Regex(@"(\d+)").Replace(Version, x => $"{int.Parse(x.Groups[0].Value) + 1}");
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(newVersion));

    // act
    await _sut.Start();

    // assert
    _logger.Received().Log(LogLevel.Warning, Arg.Is<string>(s => s.Contains($"A new Repeater version ({newVersion}) is available")));
  }

  [Fact]
  public async Task Stop_RunningRepeater_Stops()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
    await _sut.Start();

    // act
    await _sut.Stop();

    // assert
    await _eventBus.Received().Publish(Arg.Is<RepeaterStatusEvent>(x => x.Status == RepeaterStatus.Disconnected && x.RepeaterId == Id));
  }

  [Fact]
  public async Task Stop_RunningRepeater_EntersIntoOff()
  {
    // arrange
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
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
    // assert
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
    await _sut.Start();
    await _sut.Stop();

    // act
    await _sut.Stop();

    // assert
    _sut.Status.Should().Be(RunningStatus.Off);
  }

  [Fact]
  public async Task Stop_StopsSendingPing()
  {
    // assert
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
    await _sut.Start();

    // act
    await _sut.Stop();

    // assert
    _timerProvider.Received().Stop();
  }

  [Fact]
  public async Task DisposeAsync_StopsRepeater()
  {
    // assert
    _eventBus.Execute(Arg.Any<RegisterRepeaterCommand>()).Returns(new RegisterRepeaterResult(Version));
    await _sut.Start();

    // act
    await _sut.DisposeAsync();

    // assert
    await _eventBus.Received().Publish(Arg.Is<RepeaterStatusEvent>(x => x.Status == RepeaterStatus.Disconnected && x.RepeaterId == Id));
  }
}
