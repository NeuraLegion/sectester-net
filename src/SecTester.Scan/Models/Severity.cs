using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Severity
{
  [EnumMember(Value = "Medium")]
  Medium,

  [EnumMember(Value = "High")]
  High,

  [EnumMember(Value = "Low")]
  Low
}
