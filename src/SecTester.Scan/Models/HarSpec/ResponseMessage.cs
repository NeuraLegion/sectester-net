namespace SecTester.Scan.Models.HarSpec;

public record ResponseMessage(int Status, string StatusText, string RedirectURL, Content Content) : EntryMessage;
