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

To establish a secure connection between the Bright cloud engine and a target on a local network, you just need to use the `IRepeaterFactory` constructed with [`Configuration` instance](https://github.com/NeuraLegion/sectester-net/tree/master/src/SecTester.Core#configuration).

```csharp
var repeaterFactory = serviceProvider.GetService<IRepeaterFactory>();
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
    var repeaterFactory = serviceProvider.GetService<IRepeaterFactory>();
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

Under the hood `Repeater` uses the `IRequestRunner` to proceed with request:

```csharp
public interface IRequestRunner
{
  Protocol Protocol
{
  get;
}

Task<IResponse> Run(IRequest request);
}
```

The package provide a single `RequestRunner` implementations for HTTP protocol. To add support for other protocols, new implementation of `IRequestRunner` should be registered in the IoC container:

```csharp
collection.AddScoped<IRequestRunner, CustomProtocolRequestRunner>();
```

## Limitations

Custom scripts and self-signed certificates (see [Bright CLI](https://www.npmjs.com/package/@brightsec/cli)) are not supported yet.

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
