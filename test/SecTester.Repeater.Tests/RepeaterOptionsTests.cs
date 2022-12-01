namespace SecTester.Repeater.Tests;

public class RepeaterOptionsTests
{
  [Fact]
  public void RepeaterOptions_NamePrefixIsTooLong_ThrowsError()
  {
    // arrange
    string namePrefix = string.Concat(Enumerable.Repeat("foo", 50));

    // act
    var act = () => new RepeaterOptions
    {
      NamePrefix = namePrefix
    };

    // assert
    act.Should().Throw<ArgumentOutOfRangeException>();
  }

  [Fact]
  public void RepeaterOptions_SetsNamePrefix()
  {
    // arrange
    const string namePrefix = "foo";

    // act
    var result = new RepeaterOptions
    {
      NamePrefix = namePrefix
    };

    // assert
    result.Should().BeEquivalentTo(
    new
    {
      NamePrefix = namePrefix
    });
  }

  [Fact]
  public void RepeaterOptions_NamePrefixIsNotPassed_KeepsDefaultValue()
  {
    // act
    var result = new RepeaterOptions();

    // assert
    result.Should().BeEquivalentTo(
      new
      {
        NamePrefix = "sectester"
      });
  }
}
