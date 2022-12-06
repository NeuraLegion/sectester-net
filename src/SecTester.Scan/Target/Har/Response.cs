namespace SecTester.Scan.Target.Har;

public record Response(Content Content) : Message
{
  public int Status { get; init; } = 200;
  public string StatusText { get; init; } = "OK";
  public string RedirectUrl { get; init; } = "";
}
