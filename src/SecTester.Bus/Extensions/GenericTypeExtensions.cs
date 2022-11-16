using System;
using System.Linq;
using SecTester.Core.Bus;

namespace SecTester.Bus.Extensions;

internal static class GenericTypeExtensions
{
  public static Type GetConcreteEventListenerType(this Type type)
  {
    var (input, output) = GetEventListenerGenericTypes(type);
    var interfaceType = typeof(EventListener<,>);

    return interfaceType.MakeGenericType(input, output);
  }

  private static Tuple<Type, Type> GetEventListenerGenericTypes(this Type type)
  {
    var genericTypes = type.GetInterfaces()
      .Where(IsEventListenerType)
      .SelectMany(it => it.GetGenericArguments())
      .ToArray();

    return new Tuple<Type, Type>(genericTypes.First(), genericTypes.Last());
  }

  private static bool IsEventListenerType(Type it)
  {
    var interfaceType = GetEventListenerType();

    return it.IsGenericType && it.GetGenericTypeDefinition() == interfaceType;
  }

  private static Type GetEventListenerType()
  {
    return typeof(EventListener<,>);
  }
}
