using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum AttackParamLocation
{
  [EnumMember(Value = "artifical-fragment")]
  ArtificalFragment,
  
  [EnumMember(Value = "artifical-query")]
  ArtificalQuery,
  
  [EnumMember(Value = "body")] 
  Body,
  
  [EnumMember(Value = "fragment")] 
  Fragment,
  
  [EnumMember(Value = "header")] 
  Header,
  
  [EnumMember(Value = "path")] 
  Path,
  
  [EnumMember(Value = "query")] 
  Query
}
