using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SecTester.Bus.Commands;
using SecTester.Bus.Dispatchers;
using SecTester.Scan.Models;

namespace SecTester.Scan.Commands;

internal record UploadHar : HttpRequest<Identifiable<string>>
{
  public UploadHar(UploadHarOptions options)
    : base("/api/v1/files", HttpMethod.Post)
  {
    Params = options.Discard
      ? new[]
      {
        new KeyValuePair<string, string>("discard", options.Discard.ToString().ToLowerInvariant())
      }
      : default;


    var content = new MultipartFormDataContent
    {
      {
        new StringContent(MessageSerializer.Serialize(options.Har), Encoding.UTF8, "application/json"), "file", options.FileName
      }
    };

    Body = content;
  }
}
