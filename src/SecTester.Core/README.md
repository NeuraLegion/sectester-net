# SecTester.Core

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Core)

The core package can be used to obtain a config including credentials from different sources, and provide a simplified
abstraction to handle events and commands.

## Setup

```bash
$ dotnet add package SecTester.Core
```

## Usage

### Configuration

First, you need to generate a new instance of `Configuration`.

```csharp
var config = new Configuration(
    hostname: "app.neuralegion.com",
    credentials: new Credentials("your API key"));
```

You can also register the configuration using the dependency injection framework providing information that will be used to construct other clients.

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddSecTesterConfig("app.neuralegion.com");
  // or
  services.AddSecTesterConfig(config);
}
```

#### Options

Configuration can be customized using the following options:

```csharp
public interface IConfigurationOptions {
  string hostname
  {
    get;
  }
  Credentials? credentials
  {
    get;
  }
  List<CredentialProvider>? credentialProviders
  {
    get;
  }
}
```

The default configuration is as follows:

```csharp
{
  credentialProviders = new List<CredentialProvider> { new EnvCredentialProvider() }
}
```

#### hostname

- type: `string`

Set the application name (domain name), that is used to establish connection with.

```csharp
var config = new Configuration(hostname: "app.neuralegion.com");
```

#### credentials

- type: `Credentials`

Set credentials to access the application.

```csharp
var config = new Configuration(
  // ...
  credentials: new Credential("your API key"));
```

More info about [setting up an API key](https://docs.brightsec.com/docs/manage-your-personal-account#manage-your-personal-api-keys-authentication-tokens)

#### credentialProviders

- type: `CredentialProvider[]`

Allows you to provide credentials and load it in runtime. The configuration will invoke one provider at a time and only
continue to the next if no credentials have been located. For example, if the process finds values defined via
the `BRIGHT_TOKEN` environment variables, the file at `.sectesterrc` will not be read.

#### EnvCredentialProvider

Use this provider to read credentials from the following environment variable: `BRIGHT_TOKEN`

If the `BRIGHT_TOKEN` environment variable is not set or contains a falsy value, it will return undefined.

```csharp
var credentialsProvider = new EnvCredentialProvider();
var config = new Configuration(
  // ...
  credentialProviders: new List<CredentialProvider> { credentialsProvider });
```

### Messages

Message is used for syncing state between SDK, application and/or external services.
This functionality is done by sending messages outside using a concrete implementation of `Dispatcher`.

Depending on the type of derived class from the `Message`, it might be addressed to only one consumer or have typically multiple consumers as well.
When a message is sent to multiple consumers, the appropriate event handler in each consumer handles the message.

The `Message` is a data-holding class, but it implements a [Visitor pattern](https://en.wikipedia.org/wiki/Visitor_pattern#:~:text=In%20object%2Doriented%20programming%20and,structures%20without%20modifying%20the%20structures.)
to allow clients to perform operations on it using a visitor class (see `Dispatcher`) without modifying the source.

For instance, you can dispatch a message in a way that is more approach you or convenient from the client's perspective.

```csharp
public record Ping : Event
{
  public readonly string Status;
}

var @event = new Ping("connected");

// using a visitor pattern
await @event.Execute(dispatcher);

// or directly
await dispatcher.execute(@event);
```

The same is applicable for the `Event`. You just need to use the `EventDispatcher` instead of `CommandDispatcher`.

Each message have a correlation ID to ensure atomicity. The regular UUID is used, but you might also want to consider other options.

### Request-response

The request-response message (aka `Command`) style is useful when you need to exchange messages between various external services.
Using `Command` you can easily ensure that the service has actually received the message and sent a response back.

To create an instance of `Command` use the abstract class as follows:

```csharp
public record RequestOptions
{
  public string Url;
  public string Method;
  public Dictionary<string, string>? headers;
  public string? Body;
}

public record RequestOutput
{
  public int Status;
  public Dictionary<string, string>? headers;
  public string? Body;
}

private record Request(RequestOptions Payload) : Command<RequestOutput>
{
  public RequestOptions Payload = Payload;
}
```

To adjust its behavior you can use next options:

| Option         | Description                                                                                  |
|:---------------| -------------------------------------------------------------------------------------------- |
| `ExpectReply`  | Indicates whether to wait for a reply. By default `true`.                                    |
| `Ttl`          | Period of time that command should be handled before being discarded. By default `10000` ms. |
| `Type`         | The name of a command. By default, it is the name of specific class.                         |
| `CorelationId` | Used to ensure atomicity while working with EventBus. By default, random UUID.               |
| `CreatedAt`    | The exact date and time the command was created.                                             |

### Publish-subscribe

When you just want to publish events without waiting for a response, it is better to use the `Event`.
The ideal use case for the publish-subscribe model is when you want to simply notify another service that a certain condition has occurred.

To create an instance of `Event` use the abstract class as follows:

```csharp
public record Issue
{
  public string Name;
  public string Details;
  public string Type;
  public string? Cvss;
  public string? Cwe;
}

private record IssueDetected(Issue Issue) : Event
{
  public Issue Issue = Issue;
}
```

To adjust its behavior you can use next options:

| Option         | Description                                                                    |
|:---------------| ------------------------------------------------------------------------------ |
| `Type`         | The name of a command. By default, it is the name of specific class.           |
| `CorelationId` | Used to ensure atomicity while working with EventBus. By default, random UUID. |
| `CreatedAt`    | The exact date and time the event was created.                                 |

To create an event handler, you should implement the `Handler` interface and use the IoC container to register a handler using the interface as a provider:

```csharp
public class IssueDetectedHandler : EventHandler<Issue>
{
  public Task<Unit> Handle(IssueDetected @event)
  {
    // implementation
    return Unit.Task;
  }
}
```

> It is not possible to register multiple event handlers for a single event pattern.

As soon as the `IssueDetected` event appears, the event handler takes a single argument, the data passed from the client (in this case, an event payload which has been sent over the network).

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
