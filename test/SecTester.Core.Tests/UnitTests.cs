namespace SecTester.Core.Tests;

public class UnitTests
{
  [Fact]
  public async Task Unit_SameValue_EqualsToEachOther()
  {
    // arrange
    var unit1 = Unit.Value;
    var unit2 = await Unit.Task;

    // assert
    unit1.Should().Be(unit2);
    (unit1 == unit2).Should().BeTrue();
    (unit1 != unit2).Should().BeFalse();
    (unit1 >= unit2).Should().BeTrue();
    (unit1 <= unit2).Should().BeTrue();
    (unit1 > unit2).Should().BeFalse();
    (unit1 < unit2).Should().BeFalse();
  }

  [Fact]
  public void Unit_BeEquitable()
  {
    // arrange
    var dictionary = new Dictionary<Unit, string>
    {
      {new Unit(), "value"},
    };

    // act
    var result = dictionary[default];

    // assert
    result.Should().Be("value");
  }

  [Fact]
  public void ToString_ReturnsParentheses()
  {
    // arrange
    var unit = Unit.Value;

    // act
    var result = unit.ToString();

    // assert
    result.Should().Be("()");
  }

  [Fact]
  public void GetHashCode_ReturnsZero()
  {
    // arrange
    var unit = Unit.Value;

    // act
    var result = unit.GetHashCode();

    // assert
    result.Should().Be(0);
  }

  [Fact]
  public void CompareTo_ReturnsZero()
  {
    // arrange
    var unit1 = new Unit();
    var unit2 = new Unit();

    // act
    var result = unit1.CompareTo(unit2);

    result.Should().Be(0);
  }

  public static object[][] ValueData()
  {
    return new[]
    {
      new object[] {new(), false},
      new object[] {"", false},
      new object[] {"()", false},
      new object[] {null!, false},
      new object[] {new Uri("https://www.google.com"), false},
      new object[] {new Unit(), true},
      new object[] {Unit.Value, true},
      new object[] {Unit.Task.Result, true},
      new object[] {default(Unit), true},
    };
  }

  public static object[][] CompareToValueData()
    => ValueData().Select(objects => new[] { objects[0] }).ToArray();

  [Theory]
  [MemberData(nameof(ValueData))]
  public void Equals_BeEqual(object input, bool expected)
  {
    // arrange
    var unit1 = Unit.Value;

    // act
    var result = unit1.Equals(input);

    // assert
    result.Should().Be(expected);
  }

  [Theory]
  [MemberData(nameof(CompareToValueData))]
  public void CompareTo_value_ReturnsZero(object value)
  {
    // arrange
    var unit1 = new Unit();

    // act
    var result = ((IComparable)unit1).CompareTo(value);

    // assert
    result.Should().Be(0);
  }
}
