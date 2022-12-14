namespace SecTester.Core.Logger;

public interface IAnsiCodeColorizer
{
  string Colorize(AnsiCodeColor ansiCodeColor, string input);
}
