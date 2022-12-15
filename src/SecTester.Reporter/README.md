# SecTester.Reporter

[![Maintainability](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/maintainability)](https://codeclimate.com/github/NeuraLegion/sectester-net/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/c92a6cb490b75c55133a/test_coverage)](https://codeclimate.com/github/NeuraLegion/sectester-net/test_coverage)
![Build Status](https://github.com/NeuraLegion/sectester-net/actions/workflows/coverage.yml/badge.svg?branch=master&event=push)
![Nuget Downloads](https://img.shields.io/nuget/dt/SecTester.Reporter)

Provide an abstraction for generating test results as part of the particular test frameworks.

## Setup

```bash
$ dotnet add package SecTester.Reporter
```

## Usage

The package exposes a `DefaultFormatter` that implements a `IFormatter` interface:

```csharp
using SecTester.Reporter;

var formatter = new DefaultFormatter();
```

To convert an issue into text, you just need to call the `Format` method:

```csharp
formatter.Format(issue);
```

<details>
<summary>Sample output</summary>

```
Issue in Bright UI:   https://app.brightsec.com/scans/djoqtSDRJYaR6sH8pfYpDX/issues/8iacauN1FH9vFvDCLoo42v
Name:                 Missing Strict-Transport-Security Header
Severity:             Low
Remediation:
Make sure to proprely set and configure headers on your application - missing strict-transport-security header
Details:
The engine detected a missing strict-transport-security header. Headers are used to outline communication and
improve security of application.
Extra Details:
● Missing Strict-Transport-Security Header
    The engine detected a missing Strict-Transport-Security header, which might cause data to be sent insecurely from the client to the server.
    Remedy:
     - Make sure to set this header to one of the following options:
        1. Strict-Transport-Security: max-age=<expire-time>
        2. Strict-Transport-Security: max-age=<expire-time>; includeSubDomains
        3. Strict-Transport-Security: max-age=<expire-time>; preload
    Resources:
     - https://www.owasp.org/index.php/OWASP_Secure_Headers_Project#hsts
    Issues found on the following URLs:
     - [GET] https://qa.brokencrystals.com/
```

</details>

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
