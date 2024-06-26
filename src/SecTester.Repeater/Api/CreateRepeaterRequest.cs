using System.Net.Http;
using System.Text;
using SecTester.Core.Commands;
using SecTester.Core.Dispatchers;

namespace SecTester.Repeater.Api;

internal record CreateRepeaterRequest : HttpRequest<RepeaterIdentity>
{
  public CreateRepeaterRequest(string name, string? description) :
    base("/api/v1/repeaters", HttpMethod.Post)
  {
    var data = new
    {
      Name = name,
      Description = description,
      ClientRole = "dev-centric"
    };
    var content = MessageSerializer.Serialize(data);
    Body = new StringContent(content, Encoding.UTF8, "application/json");
  }
}
