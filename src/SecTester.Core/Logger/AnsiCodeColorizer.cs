namespace SecTester.Core.Logger;

public interface AnsiCodeColorizer
{
  string Colorize(AnsiCodeColor ansiCodeColor, string input);
}
