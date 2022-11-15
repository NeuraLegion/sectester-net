using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum ScanStatus
{
  [EnumMember(Value = "failed")]
  Failed,
  
  [EnumMember(Value = "disrupted")]
  Disrupted,
  
  [EnumMember(Value = "running")]
  Running,
  
  [EnumMember(Value = "stopped")]
  Stopped,
  
  [EnumMember(Value = "queued")]
  Queued,
  
  [EnumMember(Value = "scheduled")]
  Scheduled,
  
  [EnumMember(Value = "pending")]
  Pending,
  
  [EnumMember(Value = "done")]
  Done,
  
  [EnumMember(Value = "paused")]
  Paused
}
