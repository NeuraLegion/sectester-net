using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecTester.Core;
using SecTester.Core.Utils;
using SecTester.Scan.Models;
using SecTester.Scan.Models.HarSpec;

namespace SecTester.Scan;

public class DefaultScanFactory : ScanFactory
{
  internal const int MaxSlugLength = 200;

  private static readonly IEnumerable<Discovery> DefaultDiscoveryTypes = new List<Discovery>
  {
    Discovery.Archive
  };

  private readonly Configuration _configuration;
  private readonly ILoggerFactory _loggerFactory;

  private readonly Scans _scans;
  private readonly SystemTimeProvider _systemTimeProvider;

  public DefaultScanFactory(Configuration configuration, Scans scans, ILoggerFactory loggerFactory,
    SystemTimeProvider systemTimeProvider)
  {
    _scans = scans ?? throw new ArgumentNullException(nameof(scans));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _systemTimeProvider = systemTimeProvider ?? throw new ArgumentNullException(nameof(systemTimeProvider));
    _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
  }

  public async Task<IScan> CreateScan(ScanSettings settings, ScanOptions? options)
  {
    var scanConfig = await BuildScanConfig(settings).ConfigureAwait(false);
    var scanId = await _scans.CreateScan(scanConfig).ConfigureAwait(false);

    return new Scan(scanId, _scans, _loggerFactory.CreateLogger<Scan>(), options ?? new ScanOptions());
  }

  private async Task<ScanConfig> BuildScanConfig(ScanSettings scanSettings)
  {
    var fileId = await CreateAndUploadHar(scanSettings.Target).ConfigureAwait(false);

    return new ScanConfig(scanSettings.Name)
    {
      FileId = fileId,
      Smart = scanSettings.Smart,
      PoolSize = scanSettings.PoolSize,
      SkipStaticParams = scanSettings.SkipStaticParams,
      Module = Module.Dast,
      DiscoveryTypes = DefaultDiscoveryTypes,
      AttackParamLocations = scanSettings.AttackParamLocations,
      Tests = scanSettings.Tests,
      Repeaters = scanSettings.RepeaterId is null
        ? default
        : new List<string>
        {
          scanSettings.RepeaterId
        },
      SlowEpTimeout =
        scanSettings.SlowEpTimeout is null ? default : (int)scanSettings.SlowEpTimeout.Value.TotalSeconds,
      TargetTimeout =
        scanSettings.TargetTimeout is null ? default : (int)scanSettings.TargetTimeout.Value.TotalSeconds
    };
  }

  private async Task<string> CreateAndUploadHar(Target target)
  {
    var filename = GenerateFileName(target.Url);
    var har = await CreateHar(target).ConfigureAwait(false);

    return await _scans.UploadHar(new UploadHarOptions(har, filename, true)).ConfigureAwait(false);
  }

  private static string GenerateFileName(string url)
  {
    var host = new Uri(url).Host;

    host = host.Length <= MaxSlugLength ? host : host.Substring(0, MaxSlugLength);

    return $"{host.TrimEnd(".-".ToCharArray())}-{Guid.NewGuid()}.har";
  }

  private async Task<Entry> CreateHarEntry(Target target)
  {
    return new Entry(_systemTimeProvider.Now,
      await target.ToHarRequest().ConfigureAwait(false),
      new ResponseMessage(200, "OK", "", new Content(-1, "text/plain"))
      {
        HttpVersion = "HTTP/1.1"
      },
      new Timings(),
      new Cache()
    );
  }

  private async Task<Har> CreateHar(Target target)
  {
    var entry = await CreateHarEntry(target).ConfigureAwait(false);

    return new Har(
      new Log(
        new Tool(_configuration.Name, _configuration.Version))
      {
        Entries = new List<Entry>
        {
          entry
        }
      }
    );
  }
}
