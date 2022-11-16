namespace SecTester.Bus.Dispatchers;

internal sealed record ConsumedMessage
{
  public string? CorrelationId { get; init; }
  public string? Name { get; init; }
  public string? Payload { get; init; }
  public string? ReplyTo { get; init; }
}
