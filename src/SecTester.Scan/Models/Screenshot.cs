using System;

namespace SecTester.Scan.Models;

public record Screenshot(string Url, string Title)
{
  public string Url { get; init; } = Url ?? throw new ArgumentNullException(nameof(Url));
  public string Title { get; init; } = Title ?? throw new ArgumentNullException(nameof(Title));
}
