using NSubstitute.ExceptionExtensions;
using SecTester.Core;
using SecTester.Repeater;
using SecTester.Repeater.Api;

namespace SecTester.Runner.Tests;

public class SecRunnerTests
{
  private const string Hostname = "app.neuralegion.com";
  private const string Token = "0zmcwpe.nexr.0vlon8mp7lvxzjuvgjy88olrhadhiukk";
  private const string RepeaterId = "123";

  private readonly Configuration _configuration = new(Hostname, new Credentials(Token));
  private readonly Formatter _formatter = Substitute.For<Formatter>();
  private readonly RepeaterOptions _options = new();
  private readonly IRepeater _repeater = Substitute.For<IRepeater>();
  private readonly RepeaterFactory _repeaterFactory = Substitute.For<RepeaterFactory>();
  private readonly Repeaters _repeatersManager = Substitute.For<Repeaters>();
  private readonly ScanFactory _scanFactory = Substitute.For<ScanFactory>();
  private readonly SecRunner _sut;

  public SecRunnerTests()
  {
    _repeater.RepeaterId.Returns(RepeaterId);
    _repeaterFactory.CreateRepeater(_options).Returns(_repeater);
    _sut = new SecRunner(_configuration, _repeaterFactory, _scanFactory, _repeatersManager, _formatter);
  }

  [Fact]
  public async Task Create_CreatesCompositeRoot()
  {
    // act
    await using var secRunner = SecRunner.Create(_configuration);

    // assert
    secRunner.Should().BeOfType<SecRunner>();
  }

  [Fact]
  public async Task Init_StartsRepeater()
  {
    // act
    await _sut.Init(_options);

    // assert
    await _repeater.Received(1).Start();
  }

  [Fact]
  public async Task Init_RepeaterStarted_RepeaterIsDefined()
  {
    // act
    await _sut.Init(_options);

    // assert
    _sut.Should().BeEquivalentTo(new
    {
      RepeaterId
    });
  }

  [Fact]
  public async Task Init_AlreadyInitialized_ThrowsException()
  {
    // arrange
    await _sut.Init(_options);

    // act
    var act = async () => await _sut.Init(_options);

    // assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Already initialized.");
  }

  [Fact]
  public async Task Clear_StopRepeater()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.Clear();

    // assert
    await _repeater.Received(1).Stop();
  }

  [Fact]
  public async Task Clear_DeletesRepeater()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.Clear();

    // assert
    await _repeatersManager.Received(1).DeleteRepeater(RepeaterId);
  }

  [Fact]
  public async Task Clear_RepeaterRemoved_RepeaterIsNotDefined()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.Clear();

    // assert
    _sut.RepeaterId.Should().BeNull();
  }

  [Fact]
  public async Task Clear_HandlesExceptionSilently()
  {
    // arrange
    _repeater.Stop().ThrowsAsync(new Exception("something went wrong"));
    await _sut.Init(_options);

    // act
    await _sut.Clear();

    // assert
    _sut.RepeaterId.Should().BeNull();
  }

  [Fact]
  public async Task Clear_DisposesRepeater()
  {
    await _sut.Init(_options);

    // act
    await _sut.Clear();

    // assert
    await _repeater.Received(1).DisposeAsync();
  }

  [Fact]
  public async Task Clear_NotInitializedYet_DoesNothing()
  {
    // act
    await _sut.Clear();

    // assert
    _sut.RepeaterId.Should().BeNull();
  }

  [Fact]
  public async Task CreateScan_CreatesSecScan()
  {
    // arrange
    var builder = new ScanSettingsBuilder()
      .WithTests(new List<TestType>
      {
        TestType.Csrf
      });
    await _sut.Init(_options);

    // act
    var scan = _sut.CreateScan(builder);

    // assert
    scan.Should().BeOfType<SecScan>();
  }

  [Fact]
  public void CreateScan_NotInitializedYet_ThrowsException()
  {
    // arrange
    var builder = new ScanSettingsBuilder()
      .WithTests(new List<TestType>
      {
        TestType.Csrf
      });

    // act
    var act = () => _sut.CreateScan(builder);

    // assert
    act.Should().Throw<InvalidOperationException>().WithMessage("Must be initialized first.");
  }

  [Fact]
  public async Task DisposeAsync_RemoveRepeaters()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.DisposeAsync();

    // assert
    await _repeatersManager.Received(1).DeleteRepeater(RepeaterId);
  }

  [Fact]
  public async Task DisposeAsync_StopsRepeater()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.DisposeAsync();

    // assert
    await _repeater.Received(1).Stop();
  }

  [Fact]
  public async Task DisposeAsync_DisposesRepeater()
  {
    // arrange
    await _sut.Init(_options);

    // act
    await _sut.DisposeAsync();

    // assert
    await _repeater.Received(1).DisposeAsync();
  }

  [Fact]
  public async Task DisposeAsync_NoInitializedYet_DoesNothing()
  {
    // act
    await _sut.DisposeAsync();

    // assert
    await _repeater.DidNotReceive().DisposeAsync();
  }
}