using System.Threading.Tasks;

namespace SecTester.Repeater;

public interface Repeaters
{
  Task<string> CreateRepeater(string name, string? description);

  Task DeleteRepeater(string repeaterId);
}
