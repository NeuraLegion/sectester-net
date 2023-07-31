using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum TestType
{
  AngularCsti,
  BackupLocations,
  BrokenSamlAuth,
  BruteForceLogin,
  BusinessConstraintBypass,
  CommonFiles,
  CookieSecurity,
  Csrf,
  CssInjection,
  [EnumMember(Value = "cve_test")]
  Cve,
  DateManipulation,
  DefaultLoginLocation,
  DirectoryListing,
  /**
   * @deprecated Use TestType.XSS instead
   */
  DomXss,
  EmailInjection,
  ExposedCouchDbApis,
  FileUpload,
  FullPathDisclosure,
  HeaderSecurity,
  Hrs,
  HtmlInjection,
  HttpMethodFuzzing,
  HttpResponseSplitting,
  IdEnumeration,
  ImproperAssetManagement,
  InsecureTlsConfiguration,
  Jwt,
  Ldapi,
  Lfi,
  MassAssignment,
  Nosql,
  OpenBuckets,
  OpenDatabase,
  Osi,
  PromptInjection,
  ProtoPollution,
  RetireJs,
  Rfi,
  AmazonS3Takeover,
  SecretTokens,
  ServerSideJsInjection,
  Sqli,
  Ssrf,
  Ssti,
  StoredXss,
  UnvalidatedRedirect,
  VersionControlSystems,
  Wordpress,
  Xpathi,
  Xss,
  Xxe
}
