using MessagePack;
using MessagePack.Resolvers;
using SecTester.Repeater.Internal;

namespace SecTester.Repeater.Internal;

internal class DefaultMessagePackSerializerOptions
{
  internal static readonly MessagePackSerializerOptions Instance = new(
    CompositeResolver.Create(
      CompositeResolver.Create(
        new MessagePackHttpHeadersFormatter(),
        new MessagePackStringEnumMemberFormatter<Protocol>(MessagePackNamingPolicy.SnakeCase),
        new MessagePackHttpMethodFormatter()),
      StandardResolver.Instance
    )
  );
}
