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
}
