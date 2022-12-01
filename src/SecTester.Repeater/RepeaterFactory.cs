using System.Threading.Tasks;

namespace SecTester.Repeater;

public interface RepeaterFactory
{
  public Task<Repeater> CreateRepeater(RepeaterOptions options);
}
