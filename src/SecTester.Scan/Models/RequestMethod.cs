using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum RequestMethod
{
  [EnumMember(Value="GET")]
  Get,
  
  [EnumMember(Value="PUT")]
  Put,
  
  [EnumMember(Value="POST")]
  Post,
  
  [EnumMember(Value="PATCH")]
  Patch, 
  
  [EnumMember(Value="DELETE")]
  Delete,
  
  [EnumMember(Value="COPY")]
  Copy,
  
  [EnumMember(Value="HEAD")]
  Head,
  
  [EnumMember(Value="OPTIONS")]
  Options,
  
  [EnumMember(Value="LINK")]
  Link, 
  
  [EnumMember(Value="UNLINK")]
  Unlink,
  
  [EnumMember(Value="PURGE")]
  Purge,
  
  [EnumMember(Value="LOCK")]
  Lock, 
  
  [EnumMember(Value="UNLOCK")]
  Unlock,
  
  [EnumMember(Value="PROPFIND")]
  Propfind,
  
  [EnumMember(Value="VIEW")]
  View 
}
