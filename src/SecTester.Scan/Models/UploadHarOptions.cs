using System;
using SecTester.Scan.Target.HarSpec;

namespace SecTester.Scan.Models;

public record UploadHarOptions(Har Har, string FileName, bool Discard = false)
{
  public Har Har { get; init; } = Har ?? throw new ArgumentNullException(nameof(Har));
  public string FileName { get; init; } = FileName ?? throw new ArgumentNullException(nameof(FileName));
}
