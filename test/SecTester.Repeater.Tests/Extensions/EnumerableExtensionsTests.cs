namespace SecTester.Repeater.Tests.Extensions;

public class EnumerableExtensionsTests
{
  private readonly Action<int> _action = Substitute.For<Action<int>>();

  [Fact]
  public void ForEach_SourceIsNotDefined_ThrowsError()
  {
    // act
    var act = () => ((null as IEnumerable<int>)!).ForEach(_action);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*source*");
  }

  [Fact]
  public void ForEach_ActionIsNotDefined_ThrowsError()
  {
    // arrange
    IEnumerable<int> list = new List<int>
    {
      1, 2, 3
    };

    // act
    var act = () => list.ForEach(null!);

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*action*");
  }

  [Fact]
  public void ForEach_IterateOverAllElements()
  {
    // arrange
    IEnumerable<int> list = new List<int>
    {
      1, 2, 3
    };

    // act
    list.ForEach(_action);

    // assert
    _action.Received(3)(Arg.Any<int>());
  }

  [Fact]
  public void ForEach_ExecutesActionInCorrectOrder()
  {
    // arrange
    var source = Enumerable.Range(1, 10);
    var items = new List<int>();

    // act
    source.ForEach(x => items.Add(x));

    // assert
    items.Should().ContainInOrder(source);
  }

  [Fact]
  public void ForEach_DoesNothingOnEmptyEnumerable()
  {
    // arrange
    IEnumerable<int> list = new List<int>();

    // act
    list.ForEach(_action);

    // assert
    _action.DidNotReceive()(Arg.Any<int>());
  }
}
