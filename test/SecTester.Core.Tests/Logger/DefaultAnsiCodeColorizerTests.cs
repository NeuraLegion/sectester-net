namespace SecTester.Core.Tests.Logger;

public class DefaultAnsiCodeColorizerTests
{
  [Fact]
  public void Colorize_EnabledIsTrue_ReturnsInputWithColors()
  {
    // arrange
    var sut = new DefaultAnsiCodeColorizer(true);

    // act
    var result = sut.Colorize(AnsiCodeColor.White, "input");

    // assert
    result.Should().Be($"{AnsiCodeColor.White}input{AnsiCodeColor.DefaultForeground}");
  }

  [Fact]
  public void Colorize_EnabledIsFalse_ReturnsInput()
  {
    // arrange
    var sut = new DefaultAnsiCodeColorizer(false);

    // act
    var result = sut.Colorize(AnsiCodeColor.White, "input");

    // assert
    result.Should().Be("input");
  }
}
