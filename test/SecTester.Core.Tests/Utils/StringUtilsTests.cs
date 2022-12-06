namespace SecTester.Core.Tests.Utils;

public class StringUtilsTests
{
  public static readonly IEnumerable<object[]> SnakeCaseMapping = new List<object[]>
  {
    new object[] { null!, null! },
    new object[] { "", "" },
    new object[] { "lowercase", "lowercase" },
    new object[] { "UPPERCASE", "uppercase" },
    new object[] { "camelCase", "camel_case" },
    new object[] { "snake_case", "snake_case" },
    new object[] { "kebab-case", "kebab-case" },
    new object[] { "PascalCase", "pascal_case" },
    new object[] { "camelCase_with_snake", "camel_case_with_snake" },
    new object[] { "PascalCase_with_snake", "pascal_case_with_snake" },
    new object[] { "kebab-case_with_snake", "kebab-case_with_snake" },
  };

  public static readonly IEnumerable<object[]> TruncateMapping = new List<object[]>
  {
    new object[] { null!, null!, 0 },
    new object[] { "", "", 0 },
    new object[] { "a", "a", -1 },
    new object[] { "a", "…", 0 },
    new object[] { "a", "a", 1 },
    new object[] { "a", "a", 2 },
    new object[] { "aa", "…", 0 },
    new object[] { "aa", "a…", 1 },
    new object[] { "aa", "aa", 2 },
    new object[] { "aa", "aa", 3 }
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

  [Theory]
  [MemberData(nameof(TruncateMapping))]
  public void Truncate_SatisfyConversion(string input, string expected, int length)
  {
    // act
    var result = input.Truncate(length);

    // assert
    result.Should().Be(expected);
  }
}
