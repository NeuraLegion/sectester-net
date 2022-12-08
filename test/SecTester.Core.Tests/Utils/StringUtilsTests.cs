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


  public static readonly IEnumerable<object[]> TruncateInput = new List<object[]>
  {
    new object[] { "aa", 2, "aa" },
    new object[] { "test", 2, "t…" },
    new object[] { "", 2, "" },
    new object[] { "test", 0, "" },
    new object[] { "found a new issue", 6, "found " },
    new object[] { "******", 4, "****" }
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
  [MemberData(nameof(TruncateInput))]
  public void Truncate_SatisfyConversion(string inputString, int inputLength, string expected)
  {
    // act
    var result = inputString.Truncate(inputLength);

    // assert
    result.Should().Be(expected);
  }
}
