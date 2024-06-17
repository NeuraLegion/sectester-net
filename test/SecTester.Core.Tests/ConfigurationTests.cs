namespace SecTester.Core.Tests;

public class ConfigurationTests
{
  public static IEnumerable<object[]> Hostnames => new List<object[]>
  {
    new object[] { "localhost", new {  Api = "http://localhost:8000" } },
    new object[] { "localhost:8080", new {  Api = "http://localhost:8000" } },
    new object[] { "http://localhost", new {  Api = "http://localhost:8000" } },
    new object[] { "http://localhost:8080", new {  Api = "http://localhost:8000" } },
    new object[] { "127.0.0.1", new {  Api = "http://127.0.0.1:8000" } },
    new object[] { "127.0.0.1:8080", new {  Api = "http://127.0.0.1:8000" } },
    new object[] { "http://127.0.0.1", new {  Api = "http://127.0.0.1:8000" } },
    new object[] { "http://127.0.0.1:8080", new {  Api = "http://127.0.0.1:8000" } },
    new object[] { "example.com", new {  Api = "https://example.com" } },
    new object[] { "example.com:443", new {  Api = "https://example.com" } },
    new object[] { "http://example.com", new {  Api = "https://example.com" } },
    new object[]
    {
      "http://example.com:443", new {  Api = "https://example.com" }
    }
  };

  [Fact]
  public void Configuration_HostnameIsNotDefined_ThrowError()
  {
    // arrange
    const string hostname = null!;

    // act
    var act = () => new Configuration(hostname);

    // assert
    act.Should().Throw<Exception>();
  }

  [Fact]
  public void Configuration_HostnameIsInvalid_ThrowError()
  {
    // arrange
    const string hostname = ":test";

    // act
    var act = () => new Configuration(hostname);

    // assert
    act.Should().Throw<Exception>();
  }

  [Fact]
  public void Configuration_CredentialsOrCredentialProvidersNoDefined_ThrowError()
  {
    // arrange
    const string hostname = "app.brightsec.com";

    // act
    var act = () => new Configuration(hostname, credentials: null, credentialProviders: new List<ICredentialProvider>());

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

  [Fact]
  public async Task LoadCredentials_ProviderIsNotDefined_Nothing()
  {
    // arrange
    var credentials = new Credentials("weobbz5.nexa.vennegtzr2h7urpxgtksetz2kwppdgj0");
    var configuration = new Configuration(hostname: "app.brightsec.com", credentials: credentials);

    // act
    await configuration.LoadCredentials();

    // assert
    configuration.Should().BeEquivalentTo(new { Credentials = credentials });
  }

  [Fact]
  public async Task LoadCredentials_GivenProvider_LoadCredentials()
  {
    // arrange
    var сredentialProvider = Substitute.For<ICredentialProvider>();
    var credentials = new Credentials("weobbz5.nexa.vennegtzr2h7urpxgtksetz2kwppdgj0");
    var credentialProviders = new List<ICredentialProvider> { сredentialProvider };
    var configuration = new Configuration(hostname: "app.brightsec.com", credentialProviders: credentialProviders);

    сredentialProvider.Get()!.Returns(Task.FromResult(credentials));

    // act
    await configuration.LoadCredentials();

    // assert
    configuration.Should().BeEquivalentTo(new { Credentials = credentials });
  }

  [Fact]
  public async Task LoadCredentials_NoOneProviderFindCredentials_ThrowError()
  {
    // arrange
    var сredentialProvider = Substitute.For<ICredentialProvider>();
    var credentialProviders = new List<ICredentialProvider> { сredentialProvider };
    var configuration = new Configuration(hostname: "app.brightsec.com", credentialProviders: credentialProviders);

    // act
    var act = () => configuration.LoadCredentials();

    // assert
    await act.Should().ThrowAsync<Exception>();
  }

  [Fact]
  public async Task LoadCredentials_MultipleProviders_SetCredentialsFromFirst()
  {
    // arrange
    var сredentialProvider = Substitute.For<ICredentialProvider>();
    var credentials1 = new Credentials("weobbz5.nexa.vennegtzr2h7urpxgtksetz2kwppdgj0");
    var credentials2 = new Credentials("weobbz5.nexa.vennegtzr2h7urpxgtksetz2kwppdgj1");
    var credentialProviders = new List<ICredentialProvider> { сredentialProvider, сredentialProvider, сredentialProvider };
    var configuration = new Configuration(hostname: "app.brightsec.com", credentialProviders: credentialProviders);

    сredentialProvider.Get()!.Returns(Task.FromResult<Credentials?>(null)!, Task.FromResult(credentials1), Task.FromResult(credentials2));

    // act
    await configuration.LoadCredentials();

    // assert
    configuration.Should().BeEquivalentTo(new { Credentials = credentials1 });
  }

  [Fact]
  public void Constructor_LogLevelOmitted_SetLogLevelToError()
  {
    // act
    var configuration = new Configuration(hostname: "app.brightsec.com");

    // assert
    configuration.LogLevel.Should().Be(LogLevel.Error);
  }

  [Fact]
  public void Constructor_GivenSpecificLogLevel_SetLogLevelToValue()
  {
    // act
    var configuration = new Configuration(hostname: "app.brightsec.com", logLevel: LogLevel.Trace);

    // assert
    configuration.LogLevel.Should().Be(LogLevel.Trace);
  }

  [Fact]
  public void Version_ReturnsSemVerProductVersion()
  {
    // act
    var configuration = new Configuration(hostname: "app.brightsec.com", logLevel: LogLevel.Trace);

    // assert
    configuration.Version.Should().MatchRegex(@"^\d+\.\d+\.\d+$");
  }
}
