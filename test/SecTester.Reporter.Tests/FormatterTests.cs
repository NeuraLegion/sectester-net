namespace SecTester.Reporter.Tests;

public class FormatterTests
{
  private const string Expected =
    @"Issue in Bright UI:   http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9
                                      Name:                 Database connection crashed
                                      Severity:             Medium
                                      Remediation:
                                      The best way to protect against those kind of issues is making sure the Database resources are sufficient
                                      Details:
                                      Cross-site request forgery is a type of malicious website exploit.";
  private readonly Formatter _sut = Substitute.For<Formatter>();

  [Fact]
  public void Format_GivenIssue_ReturnsFormattedString()
  {
    // arrange
    var issue = new Issue("pDzxcEXQC8df1fcz1QwPf9", "Cross-site request forgery is a type of malicious website exploit.",
      "Database connection crashed",
      "The best way to protect against those kind of issues is making sure the Database resources are sufficient",
      new Request("https://brokencrystals.com/"), new Request("https://brokencrystals.com/"),
      "http://app.neuralegion.com/scans/pDzxcEXQC8df1fcz1QwPf9/issues/pDzxcEXQC8df1fcz1QwPf9", 1, Severity.Medium, Protocol.Http,
      DateTime.Today);
    _sut.Format(issue).Returns(Expected);

    // act
    var result = _sut.Format(issue);

    // assert
    result.Should().Be(Expected);
  }
}


