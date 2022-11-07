namespace SecTester.Core.Tests;

public class ConfigurationTests
{
  public static IEnumerable<object[]> Hostnames => new List<object[]>
  {
    new object[] { "localhost", new { Bus = "amqp://localhost:5672", Api = "http://localhost:8000" } },
    new object[] { "localhost:8080", new { Bus = "amqp://localhost:5672", Api = "http://localhost:8000" } },
    new object[] { "http://localhost", new { Bus = "amqp://localhost:5672", Api = "http://localhost:8000" } },
    new object[] { "http://localhost:8080", new { Bus = "amqp://localhost:5672", Api = "http://localhost:8000" } },
    new object[] { "127.0.0.1", new { Bus = "amqp://127.0.0.1:5672", Api = "http://127.0.0.1:8000" } },
    new object[] { "127.0.0.1:8080", new { Bus = "amqp://127.0.0.1:5672", Api = "http://127.0.0.1:8000" } },
    new object[] { "http://127.0.0.1", new { Bus = "amqp://127.0.0.1:5672", Api = "http://127.0.0.1:8000" } },
    new object[] { "http://127.0.0.1:8080", new { Bus = "amqp://127.0.0.1:5672", Api = "http://127.0.0.1:8000" } },
    new object[] { "example.com", new { Bus = "amqps://amq.example.com:5672", Api = "https://example.com" } },
    new object[] { "example.com:443", new { Bus = "amqps://amq.example.com:5672", Api = "https://example.com" } },
    new object[] { "http://example.com", new { Bus = "amqps://amq.example.com:5672", Api = "https://example.com" } },
    new object[]
    {
      "http://example.com:443", new { Bus = "amqps://amq.example.com:5672", Api = "https://example.com" }
    }
  };

  [Fact]
  public void Configuration_HostnameIsNotDefined_ThrowError()
  {
    // arrange
    const string hostname = null!;

    // act
    Action act = () => new Configuration(hostname);

    // assert
    act.Should().Throw<Exception>();
  }

  [Fact]
  public void Configuration_HostnameIsInvalid_ThrowError()
  {
    // arrange
    const string hostname = ":test";

    // act
    Action act = () => new Configuration(hostname);

    // assert
    act.Should().Throw<Exception>();
  }

  [Theory]
  [MemberData(nameof(Hostnames))]
  public void Configuration_ValidHostname_ResolveApiAndBus(string input, object address)
  {
    // act
    var configuration = new Configuration(input);

    // assert
    configuration.Should().BeEquivalentTo(address);
  }
}
