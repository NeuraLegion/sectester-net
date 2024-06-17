using System.Runtime.Serialization;

namespace SecTester.Core.Tests.Fixtures;

internal enum FooEnum
{
  Bar,
  FooBar,
  [EnumMember(Value = "baz-qux")]
  BazQux
}
