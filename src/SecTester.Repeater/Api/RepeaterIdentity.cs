using System;

namespace SecTester.Repeater.Api;

internal record RepeaterIdentity(string Id)
{
  public string Id { get; } = Id ?? throw new ArgumentNullException(nameof(Id));
}
