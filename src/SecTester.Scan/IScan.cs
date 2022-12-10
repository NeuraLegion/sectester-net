using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public interface IScan : IAsyncDisposable
{
  string Id { get; }
  Task<IEnumerable<Issue>> Issues();
  Task Stop();
  IAsyncEnumerable<ScanState> Status(CancellationToken cancellationToken = default);
  Task Expect(Severity expectation, CancellationToken cancellationToken = default);
  Task Expect(Func<IScan, Task<bool>> predicate, CancellationToken cancellationToken = default);
}

