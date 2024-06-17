using System;

namespace SecTester.Repeater;

public record RepeaterOptions
{
  private const int MaxPrefixLength = 44;
  private readonly string _namePrefix = System.Net.Dns.GetHostName();

  public string NamePrefix
  {
    get => _namePrefix;
    init
    {
      if (string.IsNullOrEmpty(value) || value.Length > MaxPrefixLength)
      {
        throw new ArgumentOutOfRangeException($"Name prefix must be less than {MaxPrefixLength} characters.");
      }
      _namePrefix = value;
    }
  }
}
