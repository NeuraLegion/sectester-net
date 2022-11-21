using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record Comment(string Headline, IEnumerable<string>? Links = default, string? Text = default)
{
  public string Headline { get; init; } = Headline ?? throw new ArgumentNullException(nameof(Headline));
}
