using System.Net.Http;
using SecTester.Scan.Models;

namespace SecTester.Scan.Content;

public interface HttpContentFactory
{
  HttpContent CreateJsonContent<T>(T data);
  HttpContent CreateHarContent(UploadHarOptions options);
}
