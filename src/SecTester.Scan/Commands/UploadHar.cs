using System.Collections.Generic;
using System.Net.Http;
using SecTester.Bus.Commands;
using SecTester.Scan.Content;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record UploadHar : HttpRequest<Identifiable<string>>
{
  public UploadHar(UploadHarOptions options, HttpContentFactory httpContentFactory)
    : base("/api/v1/files", HttpMethod.Post)
  {
    Params = options.Discard
      ? new[] { new KeyValuePair<string, string>("discard", options.Discard.ToString().ToLowerInvariant()) }
      : default;

    Body = httpContentFactory.CreateHarContent(options);
  }
}
