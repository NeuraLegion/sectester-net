using SecTester.Scan.Tests.Fixtures;

namespace SecTester.Scan.Tests.Commands;

public class UploadHarTests : ScanFixture
{
  [Fact]
  public void Constructor_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");

    var expectedContent = new MultipartFormDataContent
    {
      {
        new StringContent(MessageSerializer.Serialize(options.Har), Encoding.UTF8, "application/json"),
        "file",
        "filename.har"
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
            ReadHttpContentAsString(ctx.Subject.First()).Should()
              .BeEquivalentTo(ReadHttpContentAsString(ctx.Expectation.First()));
            ctx.Subject.First().Headers.ContentType.Should()
              .BeEquivalentTo(ctx.Expectation.First().Headers.ContentType);
            ctx.Subject.Headers.ContentDisposition.Should().BeEquivalentTo(ctx.Expectation.Headers.ContentDisposition);
          })
          .When(info => info.Path.EndsWith(nameof(UploadHar.Body)))
      );
  }

  [Fact]
  public void Constructor_DiscardIsTrue_ConstructsInstance()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har", true);

    // act 
    var command = new UploadHar(options);

    // assert
    command.Should()
      .BeEquivalentTo(new { Params = new List<KeyValuePair<string, string>> { new("discard", "true") } });
  }
}
