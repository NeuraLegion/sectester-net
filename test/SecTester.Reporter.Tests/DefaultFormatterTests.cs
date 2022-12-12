namespace SecTester.Reporter.Tests;

public class DefaultFormatterTests
{
  public static readonly IEnumerable<object[]> FormatInput = new List<object[]>()
  {
    new object[]
    {
      new Issue("pDzxcEXQC8df1fcz1QwPf9", "Cross-site request forgery is a type of malicious website exploit.",
        "Database connection crashed",
        "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
        new Request("https://brokencrystals.com/"), new Request("https://brokencrystals.com/"),
        "http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9", 1, Severity.Medium, Protocol.Http,
        DateTime.Today),
      @"Issue in Bright UI:   http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9
Name:                 Database connection crashed
Severity:             Medium
Remediation:
The best way to protect against those kind of issues is making sure the Database resources are sufficient
Details:
Cross-site request forgery is a type of malicious website exploit."
    },
    new object[]
    {
      new Issue("pDzxcEXQC8df1fcz1QwPf9", "Cross-site request forgery is a type of malicious website exploit.",
        "Database connection crashed",
        "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
        new Request("https://brokencrystals.com/"), new Request("https://brokencrystals.com/"),
        "http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9", 1, Severity.Medium, Protocol.Http,
        DateTime.Today)
      {
        Comments = new List<Comment>()
        {
          new Comment("CommentHeadline", new List<string>(){"https://example.com/comment/1", "https://example.com/comment/2"}, "CommentText")
        },
        Resources = new List<string>(){"https://example.com/resource/1", "https://example.com/resource/2"}
      },
      @"Issue in Bright UI:   http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9
Name:                 Database connection crashed
Severity:             Medium
Remediation:
The best way to protect against those kind of issues is making sure the Database resources are sufficient
Details:
Cross-site request forgery is a type of malicious website exploit.
Extra Details:
● CommentHeadline
{TAB}CommentText
{TAB}Links:
{TAB}https://example.com/comment/1
{TAB}https://example.com/comment/2
References:
● https://example.com/resource/1
● https://example.com/resource/2"
    }
  };

  private readonly Formatter _sut = new DefaultFormatter();

  [Theory]
  [MemberData(nameof(FormatInput))]
  public void Format_GivenIssue_ReturnsFormattedString(Issue issue, string expected)
  {
    // arrange
    expected = expected.Replace("{TAB}", "\t", StringComparison.OrdinalIgnoreCase);

    // act
    var result = _sut.Format(issue);

    // assert
    result.Should().Be(expected);
  }
}
