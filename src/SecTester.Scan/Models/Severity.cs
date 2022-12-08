using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Severity
{
  [EnumMember(Value = "Low")]
  Low = 1,

  [EnumMember(Value = "Medium")]
  Medium = 2,

  [EnumMember(Value = "High")]
  High = 3
}
