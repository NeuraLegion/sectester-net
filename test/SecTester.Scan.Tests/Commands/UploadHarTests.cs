using SecTester.Scan.Commands;
using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class UploadHarTests : ScanFixture
{
  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");
    var content = new MultipartFormDataContent();

    HttpContentFactory.CreateHarContent(options).Returns(content);

    // act 
    var command = new UploadHar(options, HttpContentFactory);

    // assert
    command.Body.Should().Be(content);
    command.Url.Should().Be("/api/v1/files");
    command.Method.Should().Be(HttpMethod.Post);
    command.ExpectReply.Should().BeTrue();
    command.Params.Should().BeNull();
  }

  [Fact]
  public void Constructor_DiscardIsTrue_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har", true);

    // act 
    var command = new UploadHar(options, HttpContentFactory);

    // assert
    command.Params.Should().Contain(x => x.Key == "discard" && x.Value == "true");
  }
}
