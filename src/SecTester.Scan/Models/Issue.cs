using System;

namespace SecTester.Scan.Models;

public record Issue(string Id, int Order, string Details, string Name, Severity Severity, Protocol Protocol,
  string Remedy,
  DateTime Time, Request OriginalRequest, Request Request, string Link, string? Exposure = default,
  string[]? Resources = default, Comment[]? Comments = default, Screenshot[]? Screenshots = default,
  string? Cvss = default, string? Cwe = default, WebsocketFrame[]? Frames = default,
  WebsocketFrame[]? OriginalFrames = default, Response? Response = default)
{
  public string Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
  public string Details { get; init; } = Details ?? throw new ArgumentNullException(nameof(Details));
  public string Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
  public string Remedy { get; init; } = Remedy ?? throw new ArgumentNullException(nameof(Remedy));
  public Request OriginalRequest { get; init; } =
    OriginalRequest ?? throw new ArgumentNullException(nameof(OriginalRequest));
  public Request Request { get; init; } = Request ?? throw new ArgumentNullException(nameof(Request));
  public string Link { get; init; } = Link ?? throw new ArgumentNullException(nameof(Link));
}
