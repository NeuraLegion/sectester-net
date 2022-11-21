using System;
using System.Collections.Generic;

namespace SecTester.Scan.Models;

public record Issue(string Id, string Details, string Name, string Remedy, Request OriginalRequest, Request Request, string Link, int Order, Severity Severity, Protocol Protocol, DateTime Time)
{
  public string Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
  public string Details { get; init; } = Details ?? throw new ArgumentNullException(nameof(Details));
  public string Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
  public string Remedy { get; init; } = Remedy ?? throw new ArgumentNullException(nameof(Remedy));
  public Request OriginalRequest { get; init; } = OriginalRequest ?? throw new ArgumentNullException(nameof(OriginalRequest));
  public Request Request { get; init; } = Request ?? throw new ArgumentNullException(nameof(Request));
  public string Link { get; init; } = Link ?? throw new ArgumentNullException(nameof(Link));
  public string? Exposure { get; init; }
  public IEnumerable<string>? Resources { get; init; }
  public IEnumerable<Comment>? Comments { get; init; }
  public IEnumerable<Screenshot>? Screenshots { get; init; }
  public string? Cvss { get; init; }
  public string? Cwe { get; init; }
  public IEnumerable<WebsocketFrame>? Frames { get; init; }
  public IEnumerable<WebsocketFrame>? OriginalFrames { get; init; }
  public Response? Response { get; init; }
}
