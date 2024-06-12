using System.Runtime.Serialization;

namespace SecTester.Repeater.Tests.Fixtures;

internal enum FooEnum
{
  Bar,
  FooBar,
  [EnumMember(Value = "baz-qux")]
  BazQux
}
