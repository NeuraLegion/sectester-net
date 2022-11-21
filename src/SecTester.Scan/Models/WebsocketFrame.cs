namespace SecTester.Scan.Models;

public record WebsocketFrame(Frame Type, int? Status = default, string? Data = default, long? Timestamp = default);
