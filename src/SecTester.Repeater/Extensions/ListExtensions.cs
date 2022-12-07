using System;
using System.Collections.Generic;

namespace SecTester.Repeater.Extensions;

internal static class ListExtensions
{
  public static int Replace<T>(this List<T> source, T newValue, Predicate<T> predicate)
  {
    if (source == null)
    {
      throw new ArgumentNullException(nameof(source));
    }

    if (predicate == null)
    {
      throw new ArgumentNullException(nameof(predicate));
    }

    var idx = source.FindIndex(predicate);

    if (idx != -1)
    {
      source[idx] = newValue;
    }

    return idx;
  }
}

