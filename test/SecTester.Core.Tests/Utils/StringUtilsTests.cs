#pragma warning disable CS8601
namespace SecTester.Core.Tests.Utils;

public class StringUtilsTests
{
  public static readonly object[][] SnakeCaseMapping = {
    new object[] { null as object, null as object },
    new object[] { "", "" },
    new object[] { "0", "0" },
    new[] { "FOO", "foo" }, 
    new[] { "Bar", "bar" }, 
    new[] { "fooBar", "foo_bar" },
    new[] { "FooBar", "foo_bar" },
    new[] { "1FooBar", "1_foo_bar" },
    new[] { "Foo1Bar", "foo1_bar" },
    new[] { "FooBar1", "foo_bar1" },
    new[] { "Foo_Bar", "foo_bar" },
    new[] { "Foo_bar", "foo_bar" },
    new[] { "foo_Bar", "foo_bar" },
    new[] { "foo_bar", "foo_bar" },
  };

  [Theory]
  [MemberData(nameof(SnakeCaseMapping))]
  public void ToSnakeCase_SatisfyConversion(string input, string expected)
  {
    // act
    var result = input.ToSnakeCase();

    // assert
    result.Should().Be(expected);
  }
}
