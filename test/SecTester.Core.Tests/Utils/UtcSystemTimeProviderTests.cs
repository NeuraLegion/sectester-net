using SecTester.Core.Utils;

namespace SecTester.Core.Tests.Utils;

public class UtcSystemTimeProviderTests
{
  [Fact]
  public void Now_ReturnUtcTimeKind()
  {
    // arrange
    var sut = new UtcSystemTimeProvider();

    // act
    var moment = sut.Now;

    // assert
    moment.Kind.Should().Be(DateTimeKind.Utc);
  }

  [Fact]
  public void Now_ReturnCorrectMoment()
  {
    // arrange
    var sut = new UtcSystemTimeProvider();
    var before = DateTime.UtcNow;

    // act
    var moment = sut.Now;

    // assert
    moment.Should().BeOnOrAfter(before);
    moment.Should().BeOnOrBefore(DateTime.UtcNow);
  }
}
