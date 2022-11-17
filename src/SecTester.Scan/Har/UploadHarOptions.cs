using System;
using System.Text.Json.Serialization;

namespace SecTester.Scan.Har;

public record UploadHarOptions(Har Har, string FileName, bool? Discard = default)
{
  public Har Har { get; init; } = Har ?? throw new ArgumentNullException(nameof(Har));

  [JsonPropertyName("filename")]
  public string FileName { get; init; } = FileName ?? throw new ArgumentNullException(nameof(FileName));
}
