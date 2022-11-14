namespace SecTester.Core.Tests.Utils;

public class TaskUtilsTests
{
  [Fact]
  public async Task First_TasksDoesNotPassTest_ReturnNullOrDefault()
  {
    // arrange
    Task<int>[] tasks =
    {
      Task.Delay(1).ContinueWith(_ => 1),
      Task.Delay(2).ContinueWith(_ => 2),
      Task.Delay(3).ContinueWith(_ => 3)
    };

    // act
    int? result = await TaskUtils.First(tasks, _ => false);

    // assert
    result.Should().Be(0);
  }

  [Fact]
  public async Task First_TasksPassTest_ReturnValueFromFirstCompletedTask()
  {
    // arrange
    Task<int>[] tasks =
    {
      Task.Delay(1).ContinueWith(_ => 1),
      Task.Delay(2).ContinueWith(_ => 2),
      Task.Delay(3).ContinueWith(_ => 3)
    };

    // act
    int? result = await TaskUtils.First(tasks, x => x >= 2);

    // assert
    result.Should().BeGreaterOrEqualTo(2);
  }

  [Fact]
  public async Task First_NoTasksPassed_ReturnNullOrDefault()
  {
    // arrange
    Task<int>[] tasks = Array.Empty<Task<int>>();

    // act
    int? result = await TaskUtils.First(tasks, _ => true);

    // assert
    result.Should().Be(0);
  }

  [Fact]
  public async Task First_AtLeastOneTaskFailed_ThrowError()
  {
    // arrange
    Task<int>[] tasks =
    {
      Task.FromException<int>(new Exception("something went wrong")),
      Task.Delay(2).ContinueWith(_ => 2),
      Task.Delay(3).ContinueWith(_ => 3)
    };

    // act
    var act = () => TaskUtils.First(tasks, _ => true);

    // assert
    await act.Should().ThrowAsync<Exception>();
  }
}
