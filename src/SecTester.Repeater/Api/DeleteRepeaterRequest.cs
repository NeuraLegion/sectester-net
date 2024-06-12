using System.Net.Http;
using SecTester.Core;
using SecTester.Repeater.Commands;

namespace SecTester.Repeater.Api;

internal record DeleteRepeaterRequest : HttpRequest<Unit>
{
  public DeleteRepeaterRequest(string repeaterId) :
    base($"/api/v1/repeaters/{repeaterId}", HttpMethod.Delete, expectReply: false)
  {
  }
}
