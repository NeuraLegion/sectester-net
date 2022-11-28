using System;

namespace SecTester.Repeater.Api;

internal record RepeaterIdentity(string Id, string Name)
{
  public string Id { get; } = Id ?? throw new ArgumentNullException(nameof(Id));
  public string Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name));
}
