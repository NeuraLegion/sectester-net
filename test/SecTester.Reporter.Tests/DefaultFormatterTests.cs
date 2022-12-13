namespace SecTester.Reporter.Tests;

public class DefaultFormatterTests
{
  private const string CrLf = "\r\n";
  private const char BulletPoint = '●';
  private const char Tabulation = '\t';

  private static readonly Issue Issue = new("pDzxcEXQC8df1fcz1QwPf9",
    "Cross-site request forgery is a type of malicious website exploit.",
    "Database connection crashed",
    "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
    new Request("https://brokencrystals.com/"), new Request("https://brokencrystals.com/"),
    1, Severity.Medium, Protocol.Http, DateTime.Today)
  {
    Link = "https://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9"
  };

  private static readonly string FormattedText =
    @"Issue in Bright UI:   https://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9
Name:                 Database connection crashed
Severity:             Medium
Remediation:
The best way to protect against those kind of issues is making sure the Database resources are sufficient
Details:
Cross-site request forgery is a type of malicious website exploit.";

  public static readonly IEnumerable<object[]> FormatInput = new List<object[]>
  {
    new object[]
    {
      Issue,
      FormattedText
    },
    new object[]
    {
      Issue with
      {
        Comments = new List<Comment>
        {
          new("CommentHeadline", new List<string> { "https://example.com/comment/1", "https://example.com/comment/2" },
            $"CommentText-1{CrLf}CommentText-2")
        },
        Resources = new List<string> { "https://example.com/resource/1", "https://example.com/resource/2" }
      },
      $@"{FormattedText}
Extra Details:
{BulletPoint} CommentHeadline
{Tabulation}CommentText-1
{Tabulation}CommentText-2
{Tabulation}Links:
{Tabulation}https://example.com/comment/1
{Tabulation}https://example.com/comment/2
References:
{BulletPoint} https://example.com/resource/1
{BulletPoint} https://example.com/resource/2"
    }
  };

  private readonly Formatter _sut = new DefaultFormatter();

  [Theory]
  [MemberData(nameof(FormatInput))]
  public void Format_GivenIssue_ReturnsFormattedString(Issue issue, string expected)
  {
    // act
    var result = _sut.Format(issue);

    // assert
    // ADHOC: `DefaultFormatter::Format` returns CRLF/LF line endings respectively to OS environment
    result.Should().Be(expected.ReplaceLineEndings(Environment.NewLine));
  }
}
