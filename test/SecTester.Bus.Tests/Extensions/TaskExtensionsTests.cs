using SecTester.Bus.Tests.Fixtures;

namespace SecTester.Bus.Tests.Extensions;

public class TaskExtensionsTests
{
  [Fact]
  public void Cast_CastedToObjectReturnType_ReturnsCasted()
  {
    // arrange
    var task = Task.FromResult<object>(new FooBar("bar"));

    // act
    var result = task.Cast<FooBar>();

    // assert
    result.Should().BeOfType(typeof(Task<FooBar?>));
  }

  [Fact]
  public void Cast_ObjectReturnType_ReturnsCasted()
  {
    // arrange
    var task = Task.FromResult(new
    {
      Foo = "bar"
    });

    // act
    var result = task.Cast<FooBar>();

    // assert
    result.Should().BeOfType(typeof(Task<FooBar?>));
  }

  [Fact]
  public void Cast_GenericReturnType_ReturnsCastedToObject()
  {
    // arrange
    var task = Task.FromResult(new FooBar("bar"));

    // act
    var result = task.Cast<object?>();

    // assert
    result.Should().BeOfType(typeof(Task<object?>));
  }

  [Fact]
  public void Cast_NullPassed_ThrowsException()
  {
    // arrange
    var task = null as Task;

    // act
    var act = () => task!.Cast<FooBar?>();

    // assert
    act.Should().ThrowAsync<ArgumentNullException>().WithMessage("*task*");
  }

  [Fact]
  public void Cast_NonGenericReturnType_ThrowsException()
  {
    // arrange
    var task = Task.CompletedTask;

    // act
    var act = () => task.Cast<FooBar?>();

    // assert
    act.Should().ThrowAsync<ArgumentException>().WithMessage("An argument of type 'Task<T>' was expected");
  }
}
