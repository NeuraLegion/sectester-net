using SecTester.Core.Tests.Fixtures;

namespace SecTester.Core.Tests.Utils;

public class MessageUtilsTests
{
  public static readonly object[][] Types =
  {
    new object[]
    {
      typeof(TestMessage), nameof(TestMessage)
    },
    new object[]
    {
      typeof(TestMessage2), "custom"
    }
  };

  [Theory]
  [MemberData(nameof(Types))]
  public void MessageUtils_GivenType_ReturnsType(Type input, string expected)
  {
    // act
    var result = MessageUtils.GetMessageType(input);

    // assert
    result.Should().Be(expected);
  }

  [Fact]
  public void MessageUtils_GivenGenericType_ReturnsType()
  {
    // act
    var result = MessageUtils.GetMessageType<TestMessage>();

    // assert
    result.Should().Be("TestMessage");
  }

  [Fact]
  public void MessageUtils_GivenGenericTypeWithAttribute_ReturnsType()
  {
    // act
    var result = MessageUtils.GetMessageType<TestMessage2>();

    // assert
    result.Should().Be("custom");
  }
}
