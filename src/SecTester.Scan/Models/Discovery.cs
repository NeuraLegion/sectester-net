using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Discovery
{
  [EnumMember(Value = "crawler")]
  Crawler,

  [EnumMember(Value = "archive")]
  Archive,

  [EnumMember(Value = "oas")]
  Oas
}
