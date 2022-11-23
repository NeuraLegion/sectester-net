namespace SecTester.Scan.Tests.Extensions;

public static class TaskExtensions
{
  public static TResult? GetSync<T, TResult>(this T? t, Func<T, Task<TResult>> func)
  {
    return t is null ? default : Task.Run(() => func(t!)).GetAwaiter().GetResult();
  }
}
