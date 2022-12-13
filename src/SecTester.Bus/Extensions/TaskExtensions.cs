using System;
using System.Threading.Tasks;

namespace SecTester.Bus.Extensions;

internal static class TaskExtensions
{
  public static async Task<T?> Cast<T>(this Task task)
  {
    if (task is null)
    {
      throw new ArgumentNullException(nameof(task));
    }

    if (!task.GetType().IsGenericType)
    {
      throw new ArgumentException("An argument of type 'Task<T>' was expected");
    }

    await task.ConfigureAwait(false);

    var result = task.GetType().GetProperty(nameof(Task<T>.Result))?.GetValue(task);

    return (T?)result;
  }
}
