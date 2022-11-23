using System.Collections.Generic;
using System.Net.Http;
using SecTester.Bus.Commands;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

public record UploadHar : HttpRequest<Identifiable<string>>
{
  public UploadHar(MultipartFormDataContent body, bool? discard = default, int? ttl = null)
    : base("/api/v1/files", HttpMethod.Post, body: body, expectReply: true, ttl: ttl,
      @params: discard is true
        ? new[] { new KeyValuePair<string, string>("discard", discard.Value.ToString().ToLowerInvariant()) }
        : default
    )
  {
  }
}
