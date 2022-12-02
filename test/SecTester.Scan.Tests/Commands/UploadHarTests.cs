using SecTester.Bus.Dispatchers;
using SecTester.Scan.Commands;
using SecTester.Scan.Models;
using SecTester.Scan.Target.Har;
using SecTester.Scan.Tests.Extensions;

namespace SecTester.Scan.Tests.Commands;

public class UploadHarTests
{
  private const string HarFileName = "filename.har";

  private static readonly Har Har = new();

  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(Har, HarFileName);

    var expectedContent = new MultipartFormDataContent
    {
      {
        new StringContent(MessageSerializer.Serialize(options.Har), Encoding.UTF8, "application/json"),
        "file",
        HarFileName
      }
    };

    // act 
    var command = new UploadHar(options);

    command.Should()
      .BeEquivalentTo(
        new
        {
          Url = "/api/v1/files",
          Method = HttpMethod.Post,
          Params = default(IEnumerable<KeyValuePair<string, string>>?),
          ExpectReply = true,
          Body = expectedContent
        }, config => config.IncludingNestedObjects()
          .Using<MultipartFormDataContent>(ctx =>
          {
            ctx.Subject.First().ReadHttpContentAsString().Should()
              .Be(ctx.Expectation.First().ReadHttpContentAsString());
            ctx.Subject.First().Headers.ContentType.Should()
              .Be(ctx.Expectation.First().Headers.ContentType);
            ctx.Subject.Headers.ContentDisposition.Should().Be(ctx.Expectation.Headers.ContentDisposition);
          })
          .When(info => info.Path.EndsWith(nameof(UploadHar.Body)))
      );
  }

  [Fact]
  public void Constructor_DiscardIsTrue_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(Har, HarFileName, true);

    // act 
    var command = new UploadHar(options);

    // assert
    command.Should()
      .BeEquivalentTo(new { Params = new List<KeyValuePair<string, string>> { new("discard", "true") } });
  }
}
