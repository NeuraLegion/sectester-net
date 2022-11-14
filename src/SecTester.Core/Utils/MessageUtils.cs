using System;
using System.Linq;
using SecTester.Core.Bus;

namespace SecTester.Core.Utils;

public static class MessageUtils
{
  public static string GetMessageType<T>()
  {
    Type info = typeof(T);

    return GetMessageType(info);
  }

  public static string GetMessageType(Type info)
  {
    var attribute = info.GetCustomAttributes(typeof(MessageTypeAttribute), true).FirstOrDefault();
    return ((MessageTypeAttribute?)attribute)?.Name ?? info.Name;
  }
}
