using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Module
{
  [EnumMember(Value = "dast")] 
  Dast,

  [EnumMember(Value = "fuzzer")] 
  Fuzzer
}
