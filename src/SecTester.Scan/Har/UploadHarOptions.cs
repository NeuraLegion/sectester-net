using System;
using System.Text.Json.Serialization;

namespace SecTester.Scan.Har;

public class UploadHarOptions
{
  public HarContent HarContent { get; set; }

  [JsonPropertyName("filename")] 
  public string FileName { get; set; }
  public bool? Discard { get; set; }

  public UploadHarOptions(HarContent harContent, string fileName, bool? discard = default)
  {
    HarContent = harContent ?? throw new ArgumentNullException(nameof(harContent));
    FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    Discard = discard;
  }
}
