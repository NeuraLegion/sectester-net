using System.Threading.Tasks;

namespace SecTester.Scan;

public interface ScanFactory
{
  Task<Scan> CreateScan(ScanSettings settings, ScanOptions? options = default);
}
