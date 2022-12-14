using System.Threading.Tasks;

namespace SecTester.Repeater;

public interface IRepeaterFactory
{
  Task<IRepeater> CreateRepeater(RepeaterOptions? options = default);
}
