using System.Threading.Tasks;

namespace SecTester.Repeater.Runners;

public interface RequestRunner
{
  Protocol Protocol
  {
    get;
  }

  Task<Response> Run(Request request);
}
