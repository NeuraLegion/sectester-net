using MessagePack;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

[MessagePackObject]
public record OutgoingResponse : HttpMessage,  IResponse
{
  [Key("statusCode")]
  public int? StatusCode { get; set; }

  [Key("message")]
  public string? Message { get; set; }

  [Key("errorCode")]
  public string? ErrorCode { get; set; }
}
