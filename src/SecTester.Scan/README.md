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

To start scanning your application, you have to configure and than create a `ScanFactory` as follows:

```csharp

using SecTester.Core;
using SecTester.Core.Extensions;
using SecTester.Scan.Extensions;
using SecTester.Bus.Extensions;

...

var config = Configuration(hostname: "app.neuralegion.com")

ServiceCollection _services = new ServiceCollection();
_services.AddSecTesterConfig(config);
_services.AddSecTesterBus();
_services.AddSecTesterScan();

var provider = _services.BuildServiceProvider();

var scanFactory = provider.GetRequiredService<ScanFactory>();
```

To create a new scan, you have to define a target first (for details, see [here](#defining-a-target-for-attack)):

```csharp

var target = new SecTester.Scan.Target.Target("https://example.com");
```
The factory exposes the `CreateScan` method that returns a new [Scan instance](#managing-a-scan):

```csharp
using SecTester.Scan.Models;

var target = new SecTester.Scan.Target.Target("https://example.com");

var scan = scanFactory.CreateScan(new ScanSettings(
  target,
  new List<TestType>() { TestType.HeaderSecurity }));
```
Below you will find a list of parameters that can be used to configure a `Scan`:

| Option                 | Description                                                                                                                                                                                        |
|------------------------| -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
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
var target = new SecTester.Scan.Target.Target(
    "https://exmaple.com"
  );
```
If `Url` contains a query string, they will be parsed as search params:

```csharp
const target = new SecTester.Scan.Target.Target(
  "https://example.com?foo=bar"
  );
 
// foo=bar
```

If you pass a `Query` parameter, it will override these which obtained from `Url`:

```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithQuery(new Dictionary<string, string>() { { "bar", "foo" } });

// bar=foo
```

#### Method

- type: `HttpMethod`

The request method to be used when making the request, `GET` by default:

```csharp
public Target WithMethod(HttpMethod value);
public Target WithMethod(string value);
```
example:
```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithMethod(HttpMethod.Delete);
```

#### Query

- type: `IEnumerable<string, string>`

The query parameters to be sent with the request:

```csharp
public Target WithQuery(IEnumerable<KeyValuePair<string, string>> value);
public Target WithQuery(IEnumerable<KeyValuePair<string, string>> value,
    Func<IEnumerable<KeyValuePair<string, string>>, string> serializer);
```
example:

```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithQuery(new Dictionary<string, string>()
  {
    {"hello", "world"},
    {"foo", "123"}
  });

// hello=world&foo=123
```
> This will override the query string in url.

It is possible to define a custom serializer for query parameters:

```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithQuery(new Dictionary<string, string>(),
  _ => "a[0]=b&a[1]=c&a[2]=d");

// a[0]=b&a[1]=c&a[2]=d
```
#### Headers

- type: `IEnumerable<KeyValuePair<string, IEnumerable<string>>>`

The HTTP headers to be sent:

```csharp
public Target WithHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> value);
```

example:

```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithHeaders(new Dictionary<string, IEnumerable<string>>()
  {
    { "content-type", new List<string> { "application/json" } },
  });
```

#### Body

- type: `object`

The data to be sent as the request body. Makes sense only for `POST`, `PUT`, `PATCH`, and `DELETE`:

```csharp
public Target WithBody(FormUrlEncodedContent value);
public Target WithBody(MultipartContent value);
public Target WithBody(HttpContent value);
public Target WithBody(string value, string contentType);
```
example:

```csharp
var target = new SecTester.Scan.Target.Target("https://exmaple.com")
  .WithBody("text");
```

### Managing a scan

The `Scan` provides a lightweight API to revise and control the status of test execution.

For instance, to get a list of found issues, you can use the `Issues` method:

```csharp
var issues = await scan.Issues();
```

To wait for certain conditions you can use the `Expect` method:

```csharp
await scan.Expect(Severity.High);
var issues = await scan.Issues();
```

> It returns control as soon as a scan is done, timeout is gone, or an expectation is satisfied.

You can also define a custom expectation passing a function that accepts an instance of `Scan` as follows:

It might return a `Task` instance:

```csharp
await scan.Expect(async (Scan: scan) => {
  var issues = await scan.issues();

  return issues.length > 3;
});
```

You can use the `Status` method to obtain scan status, to ensure that the scan is done and nothing prevents the user to check for issues, or for other reasons:

```csharp
await foreach (var _ in scan.Status())
{
  // your code  
}

```

> This `await foreach...in` will work while a scan is active.

To stop scan, use the `Stop` method:

```ts
await scan.Stop();
```

To delete a scan, you just need to pass `DeleteOnDispose` scan option to factory while scan is created:

```csharp
var target = new SecTester.Scan.Target.Target("https://example.com");

var scan = scanFactory.CreateScan(new ScanSettings(
    target,
    new List<TestType>() { TestType.HeaderSecurity }),
  new ScanOptions() { DeleteOnDispose = true });

await using var _ = scan;

await scan.Expect(Severity.High);
```

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
