# SecTester.Bus

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Bus)

The package includes a simplified implementation of the `EventBus`, one based on `RabbitMQ`, to establish synchronous
and asynchronous communication between services and agents.

## Setup

```bash
$ dotnet add package SecTester.Bus
```

## Usage

### Overview

To use the RabbitMQ Event Bus, pass the following options object to the constructor method:

```csharp
const string repeaterId = "your Repeater ID";
var serviceProvider = new ServiceCollection()
  .AddSecTesterConfig("app.brightsec.com")
  .AddSecTesterBus(repeaterId)
  .BuildServiceProvider();

var bus = serviceProvider.GetService<IEventBus>();
```

The options are specific to the chosen transporter, the package distributes the `RabbitMQ` implementation by default.
The implementation exposes the properties described below:

| Option              | Description                                                             |
| :------------------ | ----------------------------------------------------------------------- |
| `Url`               | EventBus address.                                                       |
| `Exchange`          | Exchange name which routes a message to a particular queue.             |
| `ClientQueue`       | Queue name which your bus will listen to.                               |
| `AppQueue`          | Queue name which application will listen to.                            |
| `PrefetchCount`     | Sets the prefetch count for the channel. By default, `1`                |
| `ConnectTimeout`    | Time to wait for initial connect. By default, `30` seconds              |
| `ReconnectTime`     | The time to wait before trying to reconnect. By default, `20` seconds.  |
| `HeartbeatInterval` | The interval, in seconds, to send heartbeats. By default, `30` seconds. |
| `Username`          | The `username` to perform authentication.                               |
| `Password`          | The `password` to perform authentication.                               |

In case of unrecoverable or operational errors, you will get an exception while initial connecting.

### Subscribing to events

To subscribe an event handler to the particular event, you should register the handler in the `EventBus` as follows:

```csharp
public record Issue
{
  public string Name;
  public string Details;
  public string Type;
  public string? cvss;
  public string? cwe;
}

public record IssueDetected(Issue Payload) : Event
{
  public Issue Payload = Payload;
}

public class IssueDetectedHandler: IEventListener<Issue>
{
  public Task Handle(IssueDetected @event)
  {
    // implementation
  }
}

bus.Register<IssueDetectedHandler, IssueDetected>();
```

> ⚡ Make sure that you register the corresponding provider in the IoC. Otherwise, you
> get an error while receiving an event in the `EventBus`.

You can also override a event name using the `MessageType` attribute as follows:

```csharp
[MessageType(name: "issue-detected")]
public record IssueDetected(Issue Payload) : Event
{
  public Issue Payload = Payload;
}
```

Now the `IssueDetectedHandler` event handler listens for the `IssueDetected` event. As soon as the `IssueDetected` event
appears, the `EventBus` will call the `Handle()` method with the payload passed from the application.

To remove subscription, and removes the event handler, you have to call the `unregister()` method:

```csharp
await bus.Unregister<IssueDetectedHandler, IssueDetected>();
```

#### Publishing events through the event bus

The `IEventBus` exposes a `Publish()` method. This method publishes an event to the message broker.

```csharp
public record StatusChanged(string Status): Event
{
  public string Status = Status;
}

var event = new StatusChanged("connected");

await bus.Publish(event);
```

The `Publish()` method takes just a single argument, an instance of the derived class of the `Event`.

> ⚡ The class name should match one defined event in the application. Otherwise, you should override it by passing the
> expected name via the constructor or using the `MessageType` attribute.

For more information, please see `SecTester.Core`.

#### Executing RPC methods

The `IEventBus` exposes a `Execute()` method. This method is intended to perform a command to the application and returns
an `Task` with its response.

```csharp
public record Version(string Value)
{
  public string Value = Value;
}

public record LastVersion(Version Value)
{
  public Version Value = Value;
}

public record CheckVersion(Version Version): Command<LastVersion>
{
  public Version Version = Version;
}

var command = new CheckVersion(new Version("1.1.1"));
var response = await bus.Execute(command);
```

This method returns a `Task` which will eventually be resolved as a response message.

For instance, if you do not expect any response, you can easily make the `EventBus` resolve a `Task` immediately to
undefined:

```csharp
public record Record(Version Version) : Command<Unit>(false)
{
  public Version Version = Version;
}

var command = new Record(new Version("1.1.1"));
await bus.Execute(command);
```

The `HttpCommandDispatcher` is an alternative way to execute the commands over HTTP. To start, you should create
an `HttpCommandDispatcher` instance by passing the following options to the constructor:

```csharp
var httpDispatcher = serviceProvider.GetService<HttpCommandDispatcher>();
```

The command dispatcher can be customized using the following options:

| Option    | Description                                                                                                                                                                                                                                                                                                                                |
| --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `BaseUrl` | Base URL for your application instance, e.g. `https://app.brightsec.com`                                                                                                                                                                                                                                                                 |
| `Token`   | API key to access the API. Find out how to obtain [personal](https://docs.brightsec.com/docs/manage-your-personal-account#manage-your-personal-api-keys-authentication-tokens) and [organization](https://docs.brightsec.com/docs/manage-your-organization#manage-organization-apicli-authentication-tokens) API keys in the knowledgebase |
| `Timeout` | Time to wait for a server to send response headers (and start the response body) before aborting the request. Default 10000 ms                                                                                                                                                                                                             |

Then you have to create an instance of `HttpRequest` instead of a custom command, specifying the `Url` and `Method` in
addition to the `Body` that a command accepts by default:

```csharp
var body = JsonContent.Create(new { Foo = "bar" });
var command = new HttpRequest<Unit>(url: "/api/v1/repeaters",
  method: HttpMethods.Post,
  body: body);
```

Once it is done, you can perform a request using `HttpComandDispatcher` as follows:

```csharp
var response = await httpDispatcher.Execute(command);
```

Below you will find a list of parameters that can be used to configure a command:

| Option          | Description                                                                                |
| --------------- | ------------------------------------------------------------------------------------------ |
| `Url`           | Absolute URL or path that will be used for the request. By default, `/`                    |
| `Method`        | HTTP method that is going to be used when making the request. By default, `HttpMethod.Get` |
| `Params`        | Use to set query parameters.                                                               |
| `Body`          | Message that we want to transmit to the remote service.                                    |
| `ExpectReply`   | Indicates whether to wait for a reply. By default true.                                    |
| `Ttl`           | Period of time that command should be handled before being discarded. By default 10000 ms. |
| `Type`          | The name of a command. By default, it is the name of specific class.                       |
| `CorrelationId` | Used to ensure atomicity while working with EventBus. By default, random UUID.             |
| `CreatedAt`     | The exact date and time the command was created.                                           |

For more information, please see `SecTester.Core`.

#### Retry Strategy

For some noncritical operations, it is better to fail as soon as possible rather than retry a coupe of times.
For example, it is better to fail right after a smaller number of retries with only a short delay between retry
attempts, and display a message to the user.

By default, you can use the [Exponential backoff](https://en.wikipedia.org/wiki/Exponential_backoff) retry strategy to
retry an action when errors like `SocketException` appear.

You can implement your own to match the business requirements and the nature of the failure:

```csharp
public class CustomRetryStrategy: IRetryStrategy
{
  public async Task<TResult> Acquire<TResult>(task: Func<Task<TResult>>) {
    var times = 0;

    for (;;) {
      try
      {
        return await task();
      } catch
      {
        times++;

        if (times == 3)
        {
          throw;
        }
      }
    }
  }
}
```

Once a retry strategy is implemented, you can register it in the IoC container:

```csharp
collection.AddSingleton<IRetryStrategy, CustomRetryStrategy>();
```

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
