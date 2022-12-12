# SecTester.Scan

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Runner)

Run scanning for vulnerabilities just from your unit tests on CI phase.

## Setup

```bash
$ dotnet add package SecTester.Runner
```

## Step-by-step guide

### Configure SDK

To start writing tests, first obtain a Bright token, which is required for the access to Bright API. More info about [setting up an API key](https://docs.brightsec.com/docs/manage-your-personal-account#manage-your-personal-api-keys-authentication-tokens).

Then put obtained token into `BRIGHT_TOKEN` environment variable to make it accessible by default [`EnvCredentialProvider`](https://github.com/NeuraLegion/sectester-js/tree/master/packages/core#envcredentialprovider).

> Refer to `SecTester.Core` [documentation](https://github.com/NeuraLegion/sectester-net/tree/master/src/SecTester.Core#credentials) for the details on alternative ways of configuring credential providers.

Once it is done, create a configuration object. Single required option is Bright `Hostname` domain you are going to use, e.g. `app.neuralegion.com` as the main one:

```csharp
using SecTester.Core;

var config = new Configuration("app.neuralegion.com");
```

### Setup runner

To set up a runner, create `SecRunner` instance passing a previously created configuration as follows:

```csharp
using SecTester.Core;
using SecTester.Runner;

var config = new Configuration("app.neuralegion.com");
await using var runner = await SecRunner.Create(configuration);
```

After that, you have to initialize a `SecRunner` instance:

```ts
await runner.Init();
```

The runner is now ready to perform your tests, but you have to create a scan.

To dispose a runner, you just need to call the `Clear` or `DisposeAsync` method:

```csharp
await runner.Clear();

// or

await runner.DisposeAsync();
```

### Starting scan

To start scanning your application, first you have to create a `SecScan` instance, as shown below:

```csharp
await using var scan = await runner.CreateScan(new ScanSettingsBuilder()
    .WithTests(new List<TestType> { TestType.Xss }));
```

Below you will find a list of parameters that can be used to configure a `Scan`:

| Option                 | Description                                                                                                                                                                                        |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Target`               | The target that will be attacked. For details, see [here](#defining-a-target-for-attack).                                                                                                          |
| `Tests`                | The list of tests to be performed against the target application. [Learn more about tests](https://docs.brightsec.com/docs/vulnerability-guide)                                                    |
| `RepeaterId`           | Connects the scan to a Repeater agent, which provides secure access to local networks.                                                                                                             |
| `Smart`                | Minimize scan time by using automatic smart decisions regarding parameter skipping, detection phases, etc. Enabled by default.                                                                     |
| `SkipStaticParams`     | Use an advanced algorithm to automatically determine if a parameter has any effect on the target system's behavior when changed, and skip testing such static parameters. Enabled by default.      |
| `PoolSize`             | Sets the maximum concurrent requests for the scan, to control the load on your server. By default, `10`.                                                                                           |
| `AttackParamLocations` | Defines which part of the request to attack. By default, `body`, `query`, and `fragment`.                                                                                                          |
| `SlowEpTimeout`        | Automatically validate entry-point response time before initiating the vulnerability testing, and reduce scan time by skipping any entry-points that take too long to respond. By default, 1000ms. |
| `TargetTimeout`        | Measure timeout responses from the target application globally, and stop the scan if the target is unresponsive for longer than the specified time. By default, 5min.                              |
| `Name`                 | The scan name. The method and hostname by default, e.g. `GET example.com`.                                                                                                                         |

We provide a fluent interface for building a `ScanSettings` object. To use it, you start by creating a `ScanSettingsBuilder` instance, and then you call its methods to specify the various settings you want to use for the scan as shown above.

Finally, run a scan against your application:

```csharp
var target = new Target("https://localhost:8000/api/orders")
  .WithMethod(HttpMethod.Post)
  .WithBody(@"{ ""subject"": ""Test"", ""body"": ""<script>alert('xss')</script>"" }", "application/json");

await scan.Run(target);
```

The `Run` method takes a single argument (for details, see [here](https://github.com/NeuraLegion/sectester-net/tree/master/src/SecTester.Scan#defining-a-target-for-attack)), and returns promise that is resolved if scan finishes without any vulnerability found, and is rejected otherwise (on founding issue that meets threshold, on timeout, on scanning error).

If any vulnerabilities are found, they will be pretty-printed to stderr (depending on the testing framework) and formatted depending on chosen [`Formatter`](https://github.com/NeuraLegion/sectester-net/blob/master/src/SecTester.Reporter/Formatter.cs).

By default, each found issue will cause the scan to stop. To control this behavior you can set a severity threshold using the `Threshold` method:

```csharp
scan.Threshold(Severity.High);
```

Now found issues with severity lower than `High` will not cause the scan to stop.

Sometimes either due to scan configuration issues or target misbehave, the scan might take much more time than you expect.
In this case, you can provide a timeout for specifying maximum scan running time:

```csharp
scan.Timeout(TimeSpan.FromSeconds(30));
```

In that case after 30 seconds, if the scan isn't finishing or finding any vulnerability, it will throw an error.

### Usage sample

```csharp
using System.Configuration;
using SecTester.Runner;
using SecTester.Scan;
using SecTester.Scan.Models;

public class SecRunnerFixture : IAsyncLifetime
{
  public SecRunner Runner { get; private set; }

  public async Task InitializeAsync()
  {
    var hostname = ConfigurationManager.AppSettings["BrightHost"];
    // create a test runner
    Runner = await SecRunner.Create(new SecTester.Core.Configuration(hostname));
    // initialize a test runner
    await Runner.Init();
  }

  public async Task DisposeAsync()
  {
    if (Runner is not null)
    {
      // clean up runner
      await Runner.DisposeAsync();
    }

    GC.SuppressFinalize(this);
  }
}

public class OrdersApiTests : IClassFixture<SecRunnerFixture>, IAsyncDisposable
{
  private readonly SecRunnerFixture _fixture;
  private readonly SecScan _test;

  public OrdersApiTests(SecRunnerFixture fixture)
  {
    _fixture = fixture;
    _test = _fixture
      .Runner
      .CreateScan(new ScanSettingsBuilder()
        .WithTests(new List<TestType> { TestType.Xss }))
      .Threshold(Severity.Medium)
      .Timeout(TimeSpan.FromMinutes(5));
  }

  public async ValueTask DisposeAsync()
  {
    await _fixture.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public async Task Post_ApiOrder_ShouldNotHavePersistentXss()
  {
    var target = new Target("https://localhost:8000/api/orders")
      .WithMethod(HttpMethod.Post)
      .WithBody(@"{ ""subject"": ""Test"", ""body"": ""<script>alert('xss')</script>"" }", "application/json");

    await _test.Run(target);
  }

  [Fact]
  public async Task Get_ApiOrder_ShouldNotHaveReflectiveXss()
  {
    var target = new Target("https://localhost:8000/api/orders")
      .WithQuery(new Dictionary<string, string> { { "q", "<script>alert('xss')</script>" } });

    await _test.Run(target);
  }
}

```

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
