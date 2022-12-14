namespace SecTester.Core.Logger;

public class DefaultAnsiCodeColorizer : IAnsiCodeColorizer
{
  private readonly bool _enabled;

  public DefaultAnsiCodeColorizer(bool enabled)
  {
    _enabled = enabled;
  }

  public string Colorize(AnsiCodeColor ansiCodeColor, string input)
  {
    return !_enabled ? input : $"{ansiCodeColor}{input}{AnsiCodeColor.DefaultForeground}";
  }
}
