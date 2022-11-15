using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Frame
{
  [EnumMember(Value="outgoing")] 
  Outgoing,
  
  [EnumMember(Value="incoming")] 
  Incoming
}
