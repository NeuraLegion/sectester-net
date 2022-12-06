using System.Threading.Tasks;

namespace SecTester.Scan;

public interface ScanFactory
{
  Task<Scan> CreateScan(ScanSettingsOptions settingsOptions, ScanOptions? options = default);
}
