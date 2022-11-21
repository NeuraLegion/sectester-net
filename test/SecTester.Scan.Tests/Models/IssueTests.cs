namespace SecTester.Scan.Tests.Models;

public class IssueTests
{
  private const string Id = "id";
  private const int Order = 1;
  private const string Details = "details";
  private const string Name = "name";
  private const string Exposure = "exposure";
  private const string Cvss = "cvss";
  private const string Cwe = "cwe";
  private const Severity Severity = SecTester.Scan.Models.Severity.Low;
  private const Protocol Protocol = SecTester.Scan.Models.Protocol.Http;
  private const string Remedy = "remedy";
  private const string Link = "link";
  private readonly Request _originalRequest = new("http://example.com");
  private readonly Request _request = new("http://example.com");
  private readonly DateTime _time = DateTime.Now;

  [Fact]
  public void Constructor_WithRequiredParameters_ConstructInstance()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, Name, Remedy, _originalRequest, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().NotThrow();
  }

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // arrange
    var resources = Array.Empty<string>();
    var comments = Array.Empty<Comment>();
    var screenshots = Array.Empty<Screenshot>();
    var frames = Array.Empty<WebsocketFrame>();
    var originalFrames = Array.Empty<WebsocketFrame>();
    var response = new Response();

    // act
    var issue = new Issue(Id, Details, Name, Remedy, _originalRequest, _request, Link, Order, Severity, Protocol, _time)
    {
      Exposure = Exposure,
      Resources = resources,
      Comments = comments,
      Screenshots = screenshots,
      Cvss = Cvss,
      Cwe = Cwe,
      Frames = frames,
      OriginalFrames = originalFrames,
      Response = response
    };

    // assert
    issue.Should().BeEquivalentTo(new
    {
      Id,
      Order,
      Details,
      Name,
      Severity,
      Protocol,
      Remedy,
      Link,
      Exposure,
      Cvss,
      Cwe,
      Time = _time,
      OriginalRequest = _originalRequest,
      Request = _request,
      Resources = resources,
      Comments = comments,
      Screenshots = screenshots,
      Frames = frames,
      OriginalFrames = originalFrames,
      Response = response
    });
  }

  [Fact]
  public void Constructor_GivenNullId_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(null!, Details, Name, Remedy, _originalRequest, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*Id*");
  }

  [Fact]
  public void Constructor_GivenNullDetails_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, null!, Name, Remedy, _originalRequest, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*Details*");
  }

  [Fact]
  public void Constructor_GivenNullName_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, null!, Remedy, _originalRequest, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*Name*");
  }

  [Fact]
  public void Constructor_GivenNullRemedy_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, Name, null!, _originalRequest, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*Remedy*");
  }

  [Fact]
  public void Constructor_GivenNullOriginalRequest_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, Name, Remedy, null!, _request, Link, Order, Severity, Protocol, _time);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*OriginalRequest*");
  }

  [Fact]
  public void Constructor_GivenNullRequest_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, Name, Remedy, _originalRequest, null!, Link, Order, Severity, Protocol, _time);


    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage($"*Request*");
  }

  [Fact]
  public void Constructor_GivenNullLink_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Details, Name, Remedy, _originalRequest, _request, null!, Order, Severity, Protocol, _time);


    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*Link*");
  }
}
