using System.Threading.Tasks;

namespace SecTester.Scan;

public interface ScanFactory
{
  Task<IScan> CreateScan(ScanSettings settings, ScanOptions? options = default);
}
