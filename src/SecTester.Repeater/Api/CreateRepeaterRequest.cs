using System.Net.Http;
using System.Text;
using SecTester.Bus.Commands;
using SecTester.Bus.Dispatchers;
using SecTester.Core;

namespace SecTester.Repeater.Api;

internal record CreateRepeaterRequest : HttpRequest<Unit>
{
  public CreateRepeaterRequest(string name, string? description) :
    base("/api/v1/repeaters", HttpMethod.Post)
  {
    var data = new
    {
      Name = name,
      Description = description
    };
    var content = MessageSerializer.Serialize(data);
    Body = new StringContent(content, Encoding.UTF8, "application/json");
  }
}
