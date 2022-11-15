#pragma warning disable CS8604

using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class CommentTests
{
  [Fact]
  public void Constructor_GivenNullHeadline_ThrowError()
  {
    // act
    Action act = () => new Comment(null as string);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // arrange
    const string headline = "headline";
    const string text = "text";
    var links = new[] { "http://example.com/1" };

    // act
    var comment = new Comment(headline, text, links);

    // assert
    comment.Headline.Should().Be(headline);
    comment.Text.Should().Be(text);
    comment.Links.Should().BeEquivalentTo(links);
  }
}
