using System.Threading.Tasks;

namespace SecTester.Scan;

public interface IScanFactory
{
  Task<IScan> CreateScan(ScanSettings settings, ScanOptions? options = default);
}
