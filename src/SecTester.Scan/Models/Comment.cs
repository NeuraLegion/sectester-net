using System;

namespace SecTester.Scan.Models;

public class Comment
{
  public string Headline { get; set; }
  public string[]? Links { get; set; }
  public string? Text { get; set; }

  public Comment(string headline, string? text = default, string[]? links = default)
  {
    Headline = headline ?? throw new ArgumentNullException(nameof(headline));
    Text = text;
    Links = links;
  }
}
