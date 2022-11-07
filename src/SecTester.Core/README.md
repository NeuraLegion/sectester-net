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
public interface ConfigurationOptions {
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

## License

Copyright © 2022 [Bright Security](https://brightsec.com/).

This project is licensed under the MIT License - see the [LICENSE file](LICENSE) for details.
