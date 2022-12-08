namespace SecTester.Scan.Target.HarSpec;

public record ResponseMessage(int Status, string StatusText, string RedirectUrl, Content Content) : EntryMessage;
