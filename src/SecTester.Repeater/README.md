# SecTester.Repeater

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Repeater)

Package to manage repeaters and their lifecycle.

Repeaters are mandatory for scanning targets on a local network.
More info about [repeaters](https://docs.brightsec.com/docs/on-premises-repeater-local-agent).

## Setup

```bash
$ dotnet add package SecTester.Repeater
```

## Usage

To establish a secure connection between the Bright cloud engine and a target on a local network, you just need to use the `RepeaterFactory` constructed with [`Configuration` instance](https://github.com/NeuraLegion/sectester-js/tree/master/packages/core#configuration).

```csharp
var repeaterFactory = serviceProvider.GetService<RepeaterFactory>();
```

The factory exposes the `CreateRepeater` method that returns a new `Repeater` instance:

```csharp
await using var repeater = await repeaterFactory.CreateRepeater();
```

You can customize some properties, e.g. name prefix or description, passing options as follows:

```csharp
await using var repeater = await repeaterFactory.CreateRepeater(new RepeaterOptions {
  NamePrefix = 'my-repeater',
  Description = 'My repeater'
});
```

The `CreateRepeater` method accepts the options described below:

| Option                 | Description                                                                                           |
| :--------------------- | ----------------------------------------------------------------------------------------------------- |
| `namePrefix`           | Enter a name prefix that will be used as a constant part of the unique name. By default, `sectester`. |
| `description`          | Set a short description of the Repeater.                                                              |
| `requestRunnerOptions` | Custom the request runner settings that will be used to execute requests to your application.         |

The default `requestRunnerOptions` is as follows:

```js
{
  timeout: 30000,
    maxContentLength: 100,
    reuseConnection: false,
    allowedMimes: [
    'text/html',
    'text/plain',
    'text/css',
    'text/javascript',
    'text/markdown',
    'text/xml',
    'application/javascript',
    'application/x-javascript',
    'application/json',
    'application/xml',
    'application/x-www-form-urlencoded',
    'application/msgpack',
    'application/ld+json',
    'application/graphql'
  ]
};
```

The `RequestRunnerOptions` exposes the following options that can used to customize the request runner's behavior: [RequestRunnerOptions.cs](https://github.com/NeuraLegion/sectester-net/blob/master/src/SecTester.Repeater/Runners/RequestRunnerOptions.cs)

The `Repeater` instance provides the `Start` method. This method is required to establish a connection with the Bright cloud engine and interact with other services.

```csharp
await repeater.Start();
```

To dispose of the connection, stop accepting any incoming commands, and handle events, you can call the `Stop` method if the `Repeater` instance is started:

```csharp
await repeater.Stop();
```

`Repeater` instance also has a `RepeaterId` field, that is required to start a new scan for local targets.

### Usage in unit tests

There are multiple strategies of how to run a repeater: before-all or before-each (recommended).
The two most viable options are running before all the tests vs running before every single test.

Below you can find the implementation of before-each strategy:

```csharp
public class ScanTests: IAsyncDisposable, IAsyncLifetime
{
  // ...
  private readonly Repeater _repeater;

  public ScanTests()
  {
    // ...
    var repeaterFactory = serviceProvider.GetService<RepeaterFactory>();
    _repeater = repeaterFactory.CreateRepeater();
  }

  public async Task InitializeAsync()
  {
     await _repeater.Start();
  }

  public async ValueTask DisposeAsync()
  {
    await _repeater.DisposeAsync();

    GC.SuppressFinalize(this);
  }

  [Fact]
  public void BeNotVulnerable()
  {
    // run scan of local target passing `repeater.repeaterId` to scan config
  }
}
```

### Implementation details

Under the hood `Repeater` register `RequestExecutingEventHandler` in bus,
which in turn uses the `RequestRunner` to proceed with request:

```csharp
public interface RequestRunner
{
  Protocol Protocol
{
  get;
}

Task<Response> Run(Request request);
}
```

> We are going to provide `RequestRunner` implementations for both HTTP and WS protocols soon.

To support other protocol new class implementation of `RequestRunner` should be registered in the IoC container:

```csharp
collection.AddScoped<RequestRunner, CustomProtocolRequestRunner>();
```

## Limitations

Custom scripts and self-signed certificates (see [NexPloit CLI](https://www.npmjs.com/package/@neuralegion/nexploit-cli)) are not supported yet.

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
