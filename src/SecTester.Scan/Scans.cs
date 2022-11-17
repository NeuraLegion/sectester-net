using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SecTester.Scan.Har;
using SecTester.Scan.Models;

namespace SecTester.Scan;

public interface Scans
{
  Task<string> CreateScan(ScanConfig config);
  Task<IEnumerable<Issue>> ListIssues(string id, int? limit, string? nextId, DateTime? nextCreatedAt);
  Task StopScan(string id);
  Task DeleteScan(string id);
  Task<ScanState> GetScan(string id);
  Task<string> UploadHar(UploadHarOptions options);
}
