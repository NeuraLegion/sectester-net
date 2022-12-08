# SecTester.Scan

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Scan)

The scan package can be used to obtain a config including credentials from different sources, and provide a simplified
abstraction to handle events and commands.

## Setup

```bash
$ dotnet add package SecTester.Scan
```

## Usage

To start scanning your application, you have to configure and retrieve a `ScanFactory` as follows:

```csharp
var scanFactory = serviceProvider.GetService<ScanFactory>();
```

To create a new scan, you have to define a target first (for details, see [here](#defining-a-target-for-attack)):

```csharp
var target = new Target("https://example.com");
```

The factory exposes the `CreateScan` method that returns a new [Scan instance](#managing-a-scan):

```csharp
await using var result = scanFactory.CreateScan(new ScanSettings(
  target,
  new List<TestType>() { TestType.HeaderSecurity }));
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

### Defining a target for attack

The target can accept the following options:

#### Url

- type: `string`

The server URL that will be used for the request. Usually the `Url` represents a WHATWG URL:

```csharp
var target = new Target(
  "https://example.com"
);
```

If `Url` contains a query string, they will be parsed as search params:

```csharp
const target = new Target(
  "https://example.com?foo=bar"
);
```

If you pass a `Query` parameter, it will override these which obtained from `Url`:

```csharp
var target = new Target("https://example.com?foo=bar")
  .WithQuery(new Dictionary<string, string>() { { "bar", "foo" } });
```

#### Method

- type: `string | HttpMethod`

The request method to be used when making the request, `GET` by default:

```csharp
var target = new Target("https://example.com")
  .WithMethod(HttpMethod.Delete);
```

#### Query

- type: `IEnumerable<KeyValuePair<string, string>>`

The query parameters to be sent with the request:

```csharp
var target = new Target("https://example.com")
  .WithQuery(new Dictionary<string, string>()
  {
    {"hello", "world"},
    {"foo", "123"}
  });
```

> This will override the query string in url.

It is possible to define a custom serializer for query parameters:

```csharp
using Cysharp.Web;

var target = new Target("https://example.com")
  .WithQuery(new Dictionary<string, string>()
  {
    {"foo", "bar"},
    {"foo", "baz"}
  }, query => WebSerializer.ToQueryString(query));
```

#### Headers

- type: `IEnumerable<KeyValuePair<string, IEnumerable<string>>>`

The HTTP headers to be sent:

```csharp
var target = new Target("https://example.com")
  .WithHeaders(new Dictionary<string, IEnumerable<string>>()
  {
    { "content-type", new List<string> { "application/json" } },
  });
```

#### Body

- type: `string | HttpContent`

The data to be sent as the request body. Makes sense only for `POST`, `PUT`, `PATCH`, and `DELETE`:

```csharp
var target = new Target("https://example.com")
  .WithBody(@"{""foo"":""bar""}", "application/json");
```

You can use any derived class of `HttpContent`, such as [MultipartContent](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.multipartcontent?view=netstandard-2.0), as request body as well:

```csharp
var content = new MultipartFormDataContent {
  {
    new StringContent("Hello, world!", Encoding.UTF8, "text/plain"),
    "greeting"
  }
};
var target = new Target("https://example.com")
  .WithBody(content);
```

### Managing a scan

The `Scan` provides a lightweight API to revise and control the status of test execution.

For instance, to get a list of found issues, you can use the `issues` method:

```csharp
var issues = await scan.Issues();
```

To wait for certain conditions you can use the `expect` method:

```csharp
await scan.Expect(Severity.High);
var issues = await scan.Issues();
```

> It returns control as soon as a scan is done, timeout is gone, or an expectation is satisfied.

You can also define a custom expectation passing a function that accepts an instance of `Scan` as follows:

```csharp
await scan.Expect(async scan => {
    var issues = await scan.Issues();

    return issues.Count() > 3;
});
```

You can use the `Status` method to obtain scan status, to ensure that the scan is done and nothing prevents the user to check for issues, or for other reasons:

```csharp
await foreach (var state in scan.Status())
{
  // your code
}
```

> This `await foreach...in` will work while a scan is active.

To stop scan, use the `Stop` method:

```ts
await scan.Stop();
```

To delete a scan while disposing, you just need to set the `DeleteOnDispose` option in the `ScanOptions` as follows:

```csharp
await using var scan = scanFactory.CreateScan(settings, new ScanOptions { DeleteOnDispose = true });

await scan.Expect(Severity.High);
```

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
