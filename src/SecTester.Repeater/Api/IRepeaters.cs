using System.Threading.Tasks;

namespace SecTester.Repeater.Api;

public interface IRepeaters
{
  Task<string> CreateRepeater(string name, string? description = default);

  Task DeleteRepeater(string repeaterId);
}
