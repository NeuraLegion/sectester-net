namespace SecTester.Repeater.Tests.Extensions;

public class ListExtensionsTests
{
  [Fact]
  public void Replace_SourceIsNotDefined_ThrowsError()
  {
    // act
    var act = () => ((null as List<int>)!).Replace(4, x => x == 2);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*source*");
  }

  [Fact]
  public void Replace_PredicateIsNotDefined_ThrowsError()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 3
    };

    // act
    var act = () => list.Replace(4, null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*predicate*");
  }

  [Fact]
  public void Replace_NewValueIsNull_ReplacesItem()
  {
    // arrange
    var list = new List<int?>
    {
      1, 2, 3
    };

    // act
    list.Replace(null, x => x == 1);

    // assert
    list.Should().BeEquivalentTo(new int?[] { null, 2, 3 });
  }

  [Fact]
  public void Replace_ItemFound_ReplacesItemInList()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 3
    };

    // act
    list.Replace(4, x => x == 2);

    // assert
    list.Should().Contain(4);
  }

  [Fact]
  public void Replace_ItemFound_ReturnsIndexOfReplacedItem()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 3
    };
    var expected = list.IndexOf(2);

    // act
    var result = list.Replace(4, x => x == 2);

    // assert
    result.Should().Be(expected);
  }

  [Fact]
  public void Replace_ItemNotFound_DoesNothing()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 3
    };

    // act
    list.Replace(4, x => x == 5);

    // assert
    list.Should().BeEquivalentTo(new[]
    {
      1, 2, 3
    });
  }

  [Fact]
  public void Replace_ItemNotFound_ReturnsNegativeIndex()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 3
    };

    // act
    var result = list.Replace(4, x => x == 5);

    // assert
    result.Should().Be(-1);
  }

  [Fact]
  public void Replace_ItemFound_ReplacesFirstMatchingItem()
  {
    // arrange
    var list = new List<int>
    {
      1, 2, 1
    };
    var expected = new List<int> { 4, 2, 1 };

    // act
    list.Replace(4, x => x == 1);

    // assert
    list.Should().BeEquivalentTo(expected);
  }
}
