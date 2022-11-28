using System.Collections.Generic;
using SecTester.Bus.Commands;

namespace SecTester.Repeater.Api;

internal record ListRepeatersRequest() : HttpRequest<IEnumerable<RepeaterIdentity>>("/api/v1/repeaters/");
