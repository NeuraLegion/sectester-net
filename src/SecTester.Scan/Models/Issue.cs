using System;

namespace SecTester.Scan.Models;

public class Issue
{
  public string Id { get; set; }
  public int Order { get; set; }
  public string Details { get; set; }
  public string Name { get; set; }
  public Severity Severity { get; set; }
  public Protocol Protocol { get; set; }
  public string Remedy { get; set; }
  public DateTime Time { get; set; }
  public Request OriginalRequest { get; set; }
  public Request Request { get; set; }
  public string Link { get; set; }
  public string? Exposure { get; set; }
  public string[]? Resources { get; set; }
  public Comment[]? Comments { get; set; }
  public Screenshot[]? Screenshots { get; set; }
  public string? Cvss { get; set; }
  public string? Cwe { get; set; }
  public WebsocketFrame[]? Frames { get; set; }
  public WebsocketFrame[]? OriginalFrames { get; set; }
  public Response? Response { get; set; }

  public Issue(string id, int order, string details, string name, Severity severity, Protocol protocol, string remedy,
    DateTime time, Request originalRequest, Request request, string link, string? exposure = default,
    string[]? resources = default, Comment[]? comments = default, Screenshot[]? screenshots = default,
    string? cvss = default, string? cwe = default, WebsocketFrame[]? frames = default,
    WebsocketFrame[]? originalFrames = default, Response? response = default)
  {
    Id = id ?? throw new ArgumentNullException(nameof(id));
    Order = order;
    Details = details ?? throw new ArgumentNullException(nameof(details));
    Name = name ?? throw new ArgumentNullException(nameof(name));
    Severity = severity;
    Protocol = protocol;
    Remedy = remedy ?? throw new ArgumentNullException(nameof(remedy));
    Time = time;
    OriginalRequest = originalRequest ?? throw new ArgumentNullException(nameof(originalRequest));
    Request = request ?? throw new ArgumentNullException(nameof(request));
    Link = link ?? throw new ArgumentNullException(nameof(link));
    Exposure = exposure;
    Resources = resources;
    Comments = comments;
    Screenshots = screenshots;
    Cvss = cvss;
    Cwe = cwe;
    Frames = frames;
    OriginalFrames = originalFrames;
    Response = response;
  }
}
