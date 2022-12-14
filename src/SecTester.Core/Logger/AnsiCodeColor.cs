using System;

namespace SecTester.Core.Logger;

public class AnsiCodeColor : IEquatable<AnsiCodeColor>
{
  private readonly string _color;
  private readonly int _hashcode;

  public static readonly AnsiCodeColor DefaultForeground = new("\x1B[39m\x1B[22m");
  public static readonly AnsiCodeColor Red = new("\x1B[1m\x1B[31m");
  public static readonly AnsiCodeColor DarkRed = new("\x1B[31m");
  public static readonly AnsiCodeColor Yellow = new("\x1B[1m\x1B[33m");
  public static readonly AnsiCodeColor DarkGreen = new("\x1B[32m");
  public static readonly AnsiCodeColor White = new("\x1B[1m\x1B[37m");
  public static readonly AnsiCodeColor Cyan = new("\x1B[1m\x1B[36m");

  public AnsiCodeColor(string color)
  {
    if (string.IsNullOrEmpty(color))
    {
      throw new ArgumentNullException(nameof(color));
    }

    _color = color;
    _hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode(_color);
  }

  public override string ToString() => _color;
  public override int GetHashCode() => _hashcode;
  public override bool Equals(object obj) => Equals(obj as AnsiCodeColor);

  public bool Equals(AnsiCodeColor? other)
  {
    return other is not null && _color.Equals(other._color, StringComparison.OrdinalIgnoreCase);
  }

  public static implicit operator string(AnsiCodeColor codeColor) => codeColor.ToString();

  public static bool operator ==(AnsiCodeColor? left, AnsiCodeColor? right)
  {
    return left is null || right is null ? ReferenceEquals(left, right) : left.Equals(right);
  }

  public static bool operator !=(AnsiCodeColor? left, AnsiCodeColor? right)
  {
    return !(left == right);
  }
}
