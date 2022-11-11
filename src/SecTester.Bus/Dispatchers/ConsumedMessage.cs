using System.Diagnostics.CodeAnalysis;

namespace SecTester.Bus.Dispatchers;

[ExcludeFromCodeCoverage]
internal sealed record ConsumedMessage(string Name, object Payload, string? ReplyTo, string? CorrelationId)
{
  public string? CorrelationId { get; } = CorrelationId;
  public string Name { get; } = Name;
  public object Payload { get; } = Payload;
  public string? ReplyTo { get; } = ReplyTo;
}
