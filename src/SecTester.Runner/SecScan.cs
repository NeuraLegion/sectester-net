using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Reporter;
using SecTester.Scan;
using SecTester.Scan.Models;

namespace SecTester.Runner;

public class SecScan
{
  private readonly Formatter _formatter;
  private readonly ScanFactory _scanFactory;

  private readonly ScanSettingsBuilder _builder;
  private Severity _threshold = Severity.Low;
  private TimeSpan _timeout = TimeSpan.FromMinutes(10);

  public SecScan(ScanSettingsBuilder builder, ScanFactory scanFactory, Formatter formatter)
  {
    _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    _scanFactory = scanFactory ?? throw new ArgumentNullException(nameof(scanFactory));
    _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
  }

  public async Task Run(Target target, CancellationToken cancellationToken = default)
  {
    var scan = await CreateScan(target).ConfigureAwait(false);
    await using var _ = scan.ConfigureAwait(false);

    try
    {
      await scan.Expect(_threshold, cancellationToken).ConfigureAwait(false);

      await Assert(scan).ConfigureAwait(false);
    }
    finally
    {
      await scan.Stop().ConfigureAwait(false);
    }
  }

  public SecScan Threshold(Severity severity)
  {
    _threshold = severity;

    return this;
  }

  public SecScan Timeout(TimeSpan value)
  {
    _timeout = value;

    return this;
  }

  private async Task Assert(IScan scan)
  {
    var issue = await FindExpectedIssue(scan).ConfigureAwait(false);

    if (issue is not null)
    {
      throw new IssueFound(issue, _formatter);
    }
  }

  private async Task<Issue?> FindExpectedIssue(IScan scan)
  {
    var issues = await scan.Issues().ConfigureAwait(false);

    return issues.FirstOrDefault(issue => issue.Severity >= _threshold);
  }

  private Task<IScan> CreateScan(Target target)
  {
    var scanOptions = new ScanOptions
    {
      Timeout = _timeout
    };
    var settings = _builder.WithTarget(target).Build();

    return _scanFactory.CreateScan(settings, scanOptions);
  }
}
