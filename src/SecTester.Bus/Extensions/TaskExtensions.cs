using System;
using System.Threading.Tasks;

namespace SecTester.Bus.Extensions;

internal static class TaskExtensions
{
  public static async Task<T?> Cast<T>(this Task task)
  {
    if (task == null)
    {
      throw new ArgumentNullException(nameof(task));
    }

    if (!task.GetType().IsGenericType || task.GetType().GetGenericTypeDefinition() != typeof(Task<>))
    {
      throw new ArgumentException("An argument of type 'System.Threading.Tasks.Task`1' was expected");
    }

    await task.ConfigureAwait(false);

    var result = task.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(task);
    return (T?)result;
  }
}
