using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum AttackParamLocation
{
  [EnumMember(Value = "artifical-fragment")]
  ArtificalFragment,

  [EnumMember(Value = "artifical-query")]
  ArtificalQuery,
  Body,
  Fragment,
  Header,
  Path,
  Query
}
