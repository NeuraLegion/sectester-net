using System;
using System.Threading.Tasks;
using SecTester.Core.Bus;
using SecTester.Repeater.Runners;

namespace SecTester.Repeater.Bus;

public delegate IRequestRunner? RequestRunnerResolver(Protocol key);

public class RequestExecutingEventListener : IEventListener<RequestExecutingEvent, RequestExecutingResult>
{
  private readonly RequestRunnerResolver _requestRunnersAccessor;

  public RequestExecutingEventListener(RequestRunnerResolver requestRunnersAccessor)
  {
    _requestRunnersAccessor = requestRunnersAccessor;
  }

  public async Task<RequestExecutingResult> Handle(RequestExecutingEvent message)
  {
    var runner = _requestRunnersAccessor(message.Protocol);

    if (runner == null)
    {
      throw new InvalidOperationException($"Unsupported protocol {message.Protocol}");
    }

    return (RequestExecutingResult)(await runner.Run(message).ConfigureAwait(false));
  }
}
