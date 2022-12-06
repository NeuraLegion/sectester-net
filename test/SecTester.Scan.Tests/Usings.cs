global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Text;
global using System.Text.RegularExpressions;
global using FluentAssertions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using NSubstitute;
global using NSubstitute.ClearExtensions;
global using NSubstitute.ExceptionExtensions;
global using SecTester.Bus.Dispatchers;
global using SecTester.Bus.Extensions;
global using SecTester.Core;
global using SecTester.Core.Bus;
global using SecTester.Core.Exceptions;
global using SecTester.Core.Utils;
global using SecTester.Scan.CI;
global using SecTester.Scan.Commands;
global using SecTester.Scan.Extensions;
global using SecTester.Scan.Models;
global using SecTester.Scan.Target;
global using SecTester.Scan.Target.Har;
global using SecTester.Scan.Tests.Extensions;
global using SecTester.Scan.Tests.Mocks;
global using SecTester.Scan.Tests.Fixtures;
global using Xunit;
