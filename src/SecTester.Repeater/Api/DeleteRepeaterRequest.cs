using System.Net.Http;
using SecTester.Core;
using SecTester.Core.Commands;

namespace SecTester.Repeater.Api;

internal record DeleteRepeaterRequest : HttpRequest<Unit>
{
  public DeleteRepeaterRequest(string repeaterId) :
    base($"/api/v1/repeaters/{repeaterId}", HttpMethod.Delete, expectReply: false)
  {
  }
}
