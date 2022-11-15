namespace SecTester.Scan.Models;

public class WebsocketFrame
{
  public Frame Type { get; set; }
  public int? Status { get; set; }
  public string? Data { get; set; }
  public long? Timestamp { get; set; }

  public WebsocketFrame(Frame type, int? status = default, string? data = default, long? timestamp = default)
  {
    Type = type;
    Status = status;
    Data = data;
    Timestamp = timestamp;
  }
}
