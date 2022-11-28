using System.Collections.Generic;
using System.Threading.Tasks;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public interface Scans
{
  Task<string> CreateScan(ScanConfig config);

  Task<IEnumerable<Issue>> ListIssues(string id);

  Task StopScan(string id);

  Task DeleteScan(string id);

  Task<ScanState> GetScan(string id);

  Task<string> UploadHar(UploadHarOptions options);
}
