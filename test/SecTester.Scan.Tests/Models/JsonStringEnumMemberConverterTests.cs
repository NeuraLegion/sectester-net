using System.Runtime.Serialization;
using System.Text.Json;
using SecTester.Scan.Models;

namespace SecTester.Scan.Tests.Models;

public class JsonStringEnumMemberConverterTests
{
  public static readonly object[] Types =
  {
    new[] { typeof(Protocol) }, new[] { typeof(AttackParamLocation) }, new[] { typeof(Discovery) },
    new[] { typeof(Frame) }, new[] { typeof(RequestMethod) }, new[] { typeof(Module) }, new[] { typeof(ScanStatus) },
    new[] { typeof(Severity) }, new[] { typeof(TestType) }
  };

  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    IncludeFields = true,
    Converters = { new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase, false) }
  };

  private class Foo
  {
    public AttackParamLocation? AttackParamLocation { get; set; }
  }

  [Fact]
  public void Deserialize_GivenAnnotatedValue_ValueDeserialized()
  {
    // arrange
    var payload = @"{""attackParamLocation"":""artifical-query""}";

    // act
    var data = JsonSerializer.Deserialize<Foo>(payload, JsonSerializerOptions);

    // assert
    data.Should().NotBeNull();
    data?.AttackParamLocation.Should().Be(AttackParamLocation.ArtificalQuery);
  }

  [Fact]
  public void Deserialize_GivenNullValue_NullValueDeserialized()
  {
    // arrange
    var payload = @"{""attackParamLocation"":null}";

    // act
    var data = JsonSerializer.Deserialize<Foo>(payload, JsonSerializerOptions);

    // assert
    data.Should().NotBeNull();
    data?.AttackParamLocation.Should().BeNull();
  }

  [Fact]
  public void Serialize_GivenValue_AnnotatedValueSerialized()
  {
    // arrange
    var payload = new Foo() { AttackParamLocation = AttackParamLocation.ArtificalQuery };

    // act
    var data = JsonSerializer.Serialize(payload, JsonSerializerOptions);

    // assert
    data.Should().Be(@"{""attackParamLocation"":""artifical-query""}");
  }

  [Fact]
  public void Serialize_GivenNull_NullValueSerialized()
  {
    // arrange
    var payload = new Foo();

    // act
    var data = JsonSerializer.Serialize(payload, JsonSerializerOptions);

    // assert
    data.Should().Be(@"{""attackParamLocation"":null}");
  }

  [Theory]
  [MemberData(nameof(Types))]
  public void Enums_EnumMemberAttribute_UniqueValueAssigned(Type enumType)
  {
    // arrange
    var set = new HashSet<string>();

    // assert
    enumType.IsEnum.Should().BeTrue();

    foreach (var value in enumType.GetEnumValues())
    {
      value.Should().NotBeNull();
      var enumMemberAttribute =
        enumType.GetMember(value?.ToString() ?? "")
          .FirstOrDefault(m => m.DeclaringType == enumType)
          ?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
          .FirstOrDefault() as EnumMemberAttribute;

      enumMemberAttribute.Should()
        .NotBeNull($"{enumType} has no {nameof(EnumMemberAttribute)} defined on '{value}' member");

      set.Should().NotContain(enumMemberAttribute?.Value,
        $"{enumType} duplicates {nameof(EnumMemberAttribute)} defined on '{value}' member");

      set.Add(enumMemberAttribute?.Value ?? "");
    }
  }
}
