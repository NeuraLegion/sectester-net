global using System.Net;
global using System.Net.Http.Json;
global using System.Net.Sockets;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Timers;
global using FluentAssertions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using NSubstitute;
global using NSubstitute.ClearExtensions;
global using NSubstitute.ExceptionExtensions;
global using NSubstitute.ReturnsExtensions;
global using RichardSzalay.MockHttp;
global using SecTester.Core;
global using SecTester.Core.Bus;
global using SecTester.Core.Commands;
global using SecTester.Core.Dispatchers;
global using SecTester.Core.Exceptions;
global using SecTester.Core.RetryStrategies;
global using SecTester.Core.Logger;
global using SecTester.Core.Utils;
global using SecTester.Repeater.Api;
global using SecTester.Repeater.Bus;
global using SecTester.Repeater.Extensions;
global using SecTester.Repeater.Runners;
global using SecTester.Repeater.Tests.Mocks;
global using Xunit;

