using System;
using System.Collections.Generic;
using System.Linq;
using SecTester.Core.Bus;

namespace SecTester.Bus.Extensions;

internal static class GenericTypeExtensions
{
  public static Type GetConcreteEventListenerType(this Type type)
  {
    var genericTypes = GetEventListenerGenericTypes(type);
    var types = genericTypes as Type[] ?? genericTypes.ToArray();
    var interfaceType = typeof(EventListener<,>);

    return interfaceType.MakeGenericType(types);
  }

  private static IEnumerable<Type> GetEventListenerGenericTypes(this Type type)
  {
    var interfaceType = typeof(EventListener<,>);
    var genericTypes = type.GetInterfaces()
      .Where(it => it.IsGenericType && it.GetGenericTypeDefinition() == interfaceType)
      .SelectMany(it => it.GetGenericArguments());

    return genericTypes;
  }
}
