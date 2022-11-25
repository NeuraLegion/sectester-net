using System.Net.Http;
using System.Text;
using SecTester.Bus.Dispatchers;
using SecTester.Scan.Models;

namespace SecTester.Scan.Content;

public class DefaultHttpContentFactory : HttpContentFactory
{
  private readonly MessageSerializer _messageSerializer;

  public DefaultHttpContentFactory(MessageSerializer messageSerializer)
  {
    _messageSerializer = messageSerializer;
  }

  public virtual HttpContent CreateJsonContent<T>(T data)
  {
    return new StringContent(_messageSerializer.Serialize(data), Encoding.UTF8, "application/json");
  }

  public virtual HttpContent CreateHarContent(UploadHarOptions options)
  {
    var content = new MultipartFormDataContent();
    content.Add(CreateJsonContent(options.Har), "file", options.FileName);
    return content;
  }
}
