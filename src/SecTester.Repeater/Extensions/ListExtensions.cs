using System;
using System.Collections.Generic;

namespace SecTester.Repeater.Extensions;

internal static class ListExtensions
{
  public static int Replace<T>(this List<T> source, T newValue, Predicate<T> predicate)
  {
    if (source == null) throw new ArgumentNullException(nameof(source));
    if (predicate == null) throw new ArgumentNullException(nameof(predicate));

    var contentLenghtIdx = source.FindIndex(predicate);

    if (contentLenghtIdx != -1)
    {
      source[contentLenghtIdx] = newValue;
    }

    return contentLenghtIdx;
  }
}
