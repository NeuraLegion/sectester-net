using SecTester.Bus.Dispatchers;
using SecTester.Scan.Content;

namespace SecTester.Scan.Tests.Content;

public class DefaultHttpContentFactoryTests
{
  [Fact]
  public void CreateJsonContent_ReturnsContentWithJsonMediaType()
  {
    // arrange
    var sut = new DefaultHttpContentFactory(new DefaultMessageSerializer());

    // act
    var content = sut.CreateJsonContent(new { foo = 1 });

    // assert
    content.Should().BeOfType<StringContent>();
    content.Headers.ContentType.Should().NotBeNull();
    content.Headers.ContentType!.MediaType.Should().Be("application/json");
    content.Headers.ContentType!.CharSet.Should().Be("utf-8");
  }

  [Fact]
  public void CreateHarContent_ReturnsMultipartFormDataContent()
  {
    // arrange
    var options = new UploadHarOptions(new Har(), "filename.har");
    var nestedContent = new StringContent("");

    var sut = Substitute.ForPartsOf<DefaultHttpContentFactory>(new DefaultMessageSerializer());

    sut.CreateJsonContent(options.Har).Returns(nestedContent);

    // act
    var content = sut.CreateHarContent(options);

    // assert
    content.Should().BeOfType<MultipartFormDataContent>();
    (content as MultipartFormDataContent)!.Should().Contain(nestedContent);
    nestedContent.Headers.ContentDisposition.Should().NotBeNull();
    nestedContent.Headers.ContentDisposition!.Name.Should().Be("file");
    nestedContent.Headers.ContentDisposition!.FileName.Should().Be("filename.har");
  }
}
