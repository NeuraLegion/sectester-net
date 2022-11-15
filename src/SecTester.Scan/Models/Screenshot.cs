using System;

namespace SecTester.Scan.Models;

public class Screenshot
{
  public string Url { get; set; }
  public string Title { get; set; }

  public Screenshot(string url, string title)
  {
    Url = url ?? throw new ArgumentNullException(nameof(url));
    Title = title ?? throw new ArgumentNullException(nameof(title));
  }
}
