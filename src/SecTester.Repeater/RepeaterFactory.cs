using System.Threading.Tasks;

namespace SecTester.Repeater;

public interface RepeaterFactory
{
  Task<IRepeater> CreateRepeater(RepeaterOptions? options = default);
}
