using System.Runtime.Serialization;

namespace SecTester.Bus.Tests.Fixtures;

internal enum FooEnum
{
  Bar,
  FooBar,
  [EnumMember(Value = "baz-qux")] 
  BazQux
}
