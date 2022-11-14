namespace SecTester.Core.Tests.Utils;

public class LocalSystemTimeProviderTests
{
  [Fact]
  public void Now_ReturnLocalTimeKind()
  {
    // arrange
    var sut = new LocalSystemTimeProvider();

    // act
    var moment = sut.Now;

    // assert
    moment.Kind.Should().Be(DateTimeKind.Local);
  }

  [Fact]
  public void Now_ReturnCorrectMoment()
  {
    // arrange
    var sut = new LocalSystemTimeProvider();
    var before = DateTime.Now;

    // act
    var moment = sut.Now;

    // assert
    moment.Should().BeOnOrAfter(before);
    moment.Should().BeOnOrBefore(DateTime.Now);
  }
}
