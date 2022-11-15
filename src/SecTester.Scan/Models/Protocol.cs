using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum Protocol
{
  [EnumMember(Value = "http")] 
  Http,
  
  [EnumMember(Value = "ws")] 
  Ws
}
