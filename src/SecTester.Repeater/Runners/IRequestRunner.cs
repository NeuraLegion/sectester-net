using System.Threading.Tasks;

namespace SecTester.Repeater.Runners;

public interface IRequestRunner
{
  Protocol Protocol
  {
    get;
  }

  Task<IResponse> Run(IRequest request);
}
