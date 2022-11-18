using System.Text;

namespace SecTester.Core.Utils;

public static class StringUtils
{
  private enum SnakeCaseState
  {
    Start,
    Lower,
    Upper
  }

  public static string? ToSnakeCase(this string? name)
  {
    if (name == null || name.Length == 0)
    {
      return name;
    }

    StringBuilder sb = new StringBuilder();
    SnakeCaseState state = SnakeCaseState.Start;

    for (var i = 0; i < name.Length; ++i)
    {
      if (char.IsUpper(name[i]))
      {
        switch (state)
        {
          case SnakeCaseState.Upper:
            bool hasNext = (i + 1 < name.Length);
            if (i > 0 && hasNext)
            {
              char nextChar = name[i + 1];
              if (!char.IsUpper(nextChar) && nextChar != '_')
              {
                sb.Append('_');
              }
            }

            break;
          case SnakeCaseState.Lower:
            sb.Append('_');
            break;
        }

        char c = char.ToLowerInvariant(name[i]);
        sb.Append(c);

        state = SnakeCaseState.Upper;
      }
      else if (name[i] == '_')
      {
        sb.Append('_');
        state = SnakeCaseState.Start;
      }
      else
      {
        sb.Append(name[i]);
        state = SnakeCaseState.Lower;
      }
    }

    return sb.ToString();
  }
}
