using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecTester.Core.Utils;

public static class TaskUtils
{
  public static async Task<TResult?> First<TResult>(IEnumerable<Task<TResult>> tasks, Func<TResult?, bool> predicate)
  {
    var chain = tasks.ToList();
    var cts = new CancellationTokenSource();

    while (chain.Any())
    {
      var completedTask = await Task.WhenAny(chain).ConfigureAwait(false);

      if (completedTask.IsCompleted && predicate(completedTask.Result))
      {
        cts.Cancel();
        return completedTask.Result;
      }

      chain = chain.Where(t => t != completedTask).ToList();
    }

    return default!;
  }
}
