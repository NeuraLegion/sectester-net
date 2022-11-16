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

#### Executing RPC methods

The `HttpCommandDispatcher` is an alternative way to execute the commands over HTTP. To start, you should create
an `HttpCommandDispatcher` instance by passing the following options to the constructor:

```csharp
var collection = new ServiceCollection();
var provider = collection.BuildServiceProvider();
var httpFactory = collection.GetRequiredService<IHttpClientFactory>();
var config = collection.GetRequiredService<HttpCommandDispatcherConfig>();

var httpDispatcher = new HttpCommandDispatcher(
  httpFactory,
  config
);
```

The command dispatcher can be customized using the following options:

| Option    | Description                                                                                                                                                                                                                                                                                                                                |
| --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `BaseUrl` | Base URL for your application instance, e.g. `https://app.neuralegion.com`                                                                                                                                                                                                                                                                 |
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
var response = await httpDispatcher.execute(command);
```

Below you will find a list of parameters that can be used to configure a command:

| Option          | Description                                                                                |
| --------------- |--------------------------------------------------------------------------------------------|
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

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
