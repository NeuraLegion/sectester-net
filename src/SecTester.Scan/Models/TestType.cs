using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum TestType
{
  [EnumMember(Value = "angular_csti")] 
  AngularCsti,

  [EnumMember(Value = "backup_locations")]
  BackupLocations,

  [EnumMember(Value = "broken_saml_auth")]
  BrokenSamlAuth,

  [EnumMember(Value = "brute_force_login")]
  BruteForceLogin,

  [EnumMember(Value = "business_constraint_bypass")]
  BusinessConstraintBypass,
  
  [EnumMember(Value = "common_files")] 
  CommonFiles,

  [EnumMember(Value = "cookie_security")]
  CookieSecurity,
  
  [EnumMember(Value = "csrf")] 
  Csrf,

  [EnumMember(Value = "date_manipulation")]
  DateManipulation,

  [EnumMember(Value = "default_login_location")]
  DefaultLoginLocation,

  [EnumMember(Value = "directory_listing")]
  DirectoryListing,
  
  [EnumMember(Value = "dom_xss")] 
  DomXss,

  [EnumMember(Value = "email_injection")]
  EmailInjection,

  [EnumMember(Value = "exposed_couch_db_apis")]
  ExposedCouchDbApis,
  
  [EnumMember(Value = "file_upload")] 
  FileUpload,

  [EnumMember(Value = "full_path_disclosure")]
  FullPathDisclosure,

  [EnumMember(Value = "header_security")]
  HeaderSecurity,
  
  [EnumMember(Value = "hrs")] 
  Hrs,
  
  [EnumMember(Value = "html_injection")] 
  HtmlInjection,

  [EnumMember(Value = "http_method_fuzzing")]
  HttpMethodFuzzing,

  [EnumMember(Value = "http_response_splitting")]
  HttpResponseSplitting,
  
  [EnumMember(Value = "id_enumeration")] 
  IdEnumeration,

  [EnumMember(Value = "improper_asset_management")]
  ImproperAssetManagement,

  [EnumMember(Value = "insecure_tls_configuration")]
  InsecureTlsConfiguration,
  
  [EnumMember(Value = "jwt")] 
  Jwt,
  
  [EnumMember(Value = "ldapi")] 
  Ldapi,
  
  [EnumMember(Value = "lfi")] 
  Lfi,

  [EnumMember(Value = "mass_assignment")]
  MassAssignment,
  
  [EnumMember(Value = "nosql")] 
  Nosql,
  
  [EnumMember(Value = "open_buckets")] 
  OpenBuckets,
  
  [EnumMember(Value = "open_database")] 
  OpenDatabase,
  
  [EnumMember(Value = "osi")] 
  Osi,

  [EnumMember(Value = "proto_pollution")]
  ProtoPollution,
  
  [EnumMember(Value = "retire_js")] 
  RetireJs,
  
  [EnumMember(Value = "rfi")] 
  Rfi,
  
  [EnumMember(Value = "secret_tokens")] 
  SecretTokens,

  [EnumMember(Value = "server_side_js_injection")]
  ServerSideJsInjection,
  
  [EnumMember(Value = "sqli")] 
  Sqli,
  
  [EnumMember(Value = "ssrf")] 
  Ssrf,
  
  [EnumMember(Value = "ssti")] 
  Ssti,

  [EnumMember(Value = "version_control_systems")]
  VersionControlSystems,

  [EnumMember(Value = "wordpress")] 
  Wordpress,

  [EnumMember(Value = "xpathi")] 
  Xpathi,

  [EnumMember(Value = "xss")] 
  Xss,

  [EnumMember(Value = "xxe")] 
  Xxe
}
