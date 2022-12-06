namespace SecTester.Scan.Target.Har;

public record Response(int Status, string StatusText, string RedirectUrl, Content Content) : Message;
