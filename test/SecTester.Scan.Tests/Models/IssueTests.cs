#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class IssueTests
{
  private const string Id = "id";
  private const int Order = 1;
  private const string Details = "details";
  private const string Name = "name";
  private const Severity Severity = SecTester.Scan.Models.Severity.Low;
  private const Protocol Protocol = SecTester.Scan.Models.Protocol.Http;
  private const string Remedy = "remedy";
  private const string Link = "link";

  private readonly DateTime _time = DateTime.Now;
  private readonly Request _originalRequest = new("http://example.com");
  private readonly Request _request = new("http://example.com");

  [Fact]
  public void Constructor_WithRequiredParameters_ConstructInstance()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, Name, Severity, Protocol, Remedy, _time, _originalRequest, _request, Link);

    // assert
    act.Should().NotThrow();
  }

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // arrange
    var exposure = "exposure";
    var resources = new string[] { };
    var comments = new Comment[] { };
    var screenshots = new Screenshot[] { };
    var cvss = "cvss";
    var cwe = "cwe";
    var frames = new WebsocketFrame[] { };
    var originalFrames = new WebsocketFrame[] { };
    var response = new Response();

    // act
    var issue = new Issue(Id, Order, Details, Name, Severity, Protocol, Remedy, _time, _originalRequest, _request, Link,
      exposure,
      comments: comments,
      resources: resources,
      screenshots: screenshots,
      cvss: cvss,
      cwe: cwe,
      frames: frames,
      originalFrames: originalFrames,
      response: response);

    // assert
    issue.Id.Should().Be(Id);
    issue.Order.Should().Be(Order);
    issue.Details.Should().Be(Details);
    issue.Name.Should().Be(Name);
    issue.Severity.Should().Be(Severity);
    issue.Protocol.Should().Be(Protocol);
    issue.Remedy.Should().Be(Remedy);
    issue.Time.Should().Be(_time);
    issue.OriginalRequest.Should().Be(_originalRequest);
    issue.Request.Should().Be(_request);
    issue.Link.Should().Be(Link);
    issue.Exposure.Should().Be(exposure);
    issue.Comments.Should().BeEquivalentTo(comments);
    issue.Resources.Should().BeEquivalentTo(resources);
    issue.Screenshots.Should().BeEquivalentTo(screenshots);
    issue.Cvss.Should().Be(cvss);
    issue.Cwe.Should().Be(cwe);
    issue.Frames.Should().BeEquivalentTo(frames);
    issue.OriginalFrames.Should().BeEquivalentTo(originalFrames);
    issue.Response.Should().Be(response);
  }

  [Fact]
  public void Constructor_GivenNullId_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(null as string, Order, Details, Name, Severity, Protocol, Remedy, _time, _originalRequest, _request,
        Link);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullDetails_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, null as string, Name, Severity, Protocol, Remedy, _time, _originalRequest, _request, Link);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullName_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, null as string, Severity, Protocol, Remedy, _time, _originalRequest, _request,
        Link);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullRemedy_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, Name, Severity, Protocol, null as string, _time, _originalRequest, _request, Link);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullOriginalRequest_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, Name, Severity, Protocol, Remedy, _time, null as Request, _request, Link);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullRequest_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, Name, Severity, Protocol, Remedy, _time, _originalRequest, null as Request, Link);


    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullLink_ThrowError()
  {
    // act
    Action act = () =>
      new Issue(Id, Order, Details, Name, Severity, Protocol, Remedy, _time, _originalRequest, _request,
        null as string);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }
}
