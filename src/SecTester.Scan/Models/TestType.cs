using System;
using System.Runtime.Serialization;

namespace SecTester.Scan.Models;

public enum TestType
{
    [EnumMember(Value = "amazon_s3_takeover")]
    AmazonS3Takeover,
    
    [EnumMember(Value = "bopla")]
    BrokenObjectPropertyLevelAuthorization,
    
    [EnumMember(Value = "broken_access_control")]
    BrokenAccessControl,
    
    [EnumMember(Value = "broken_saml_auth")]
    BrokenSamlAuthentication,
    
    [EnumMember(Value = "jwt")]
    BrokenJwtAuthentication,
    
    [EnumMember(Value = "brute_force_login")]
    BruteForceLogin,
    
    [EnumMember(Value = "business_constraint_bypass")]
    BusinessConstraintBypass,
    
    [EnumMember(Value = "cookie_security")]
    CookieSecurity,
    
    [EnumMember(Value = "csrf")]
    CrossSiteRequestForgery,
    
    [EnumMember(Value = "css_injection")]
    CssInjection,
    
    [EnumMember(Value = "date_manipulation")]
    DateManipulation,
    
    [EnumMember(Value = "email_injection")]
    EmailInjection,
    
    [EnumMember(Value = "excessive_data_exposure")]
    ExcessiveDataExposure,
    
    [EnumMember(Value = "file_upload")]
    FileUpload,
    
    [EnumMember(Value = "full_path_disclosure")]
    FullPathDisclosure,
    
    [EnumMember(Value = "graphql_introspection")]
    GraphqlIntrospection,
    
    [EnumMember(Value = "html_injection")]
    HtmlInjection,
    
    [EnumMember(Value = "http_method_fuzzing")]
    HttpMethodFuzzing,
    
    [EnumMember(Value = "id_enumeration")]
    IdEnumeration,
    
    [EnumMember(Value = "iframe_injection")]
    IframeInjection,
    
    [EnumMember(Value = "improper_asset_management")]
    ImproperAssetManagement,
    
    [EnumMember(Value = "insecure_output_handling")]
    InsecureOutputHandling,
    
    [EnumMember(Value = "ldapi")]
    LdapInjection,
    
    [EnumMember(Value = "lfi")]
    LocalFileInclusion,
    
    [EnumMember(Value = "mass_assignment")]
    MassAssignment,
    
    [EnumMember(Value = "nosql")]
    MongodbInjection,
    
    [EnumMember(Value = "open_cloud_storage")]
    OpenCloudStorage,
    
    [EnumMember(Value = "open_database")]
    ExposedDatabaseDetails,
    
    [EnumMember(Value = "osi")]
    OsCommandInjection,
    
    [EnumMember(Value = "password_reset_poisoning")]
    PasswordResetPoisoning,
    
    [EnumMember(Value = "prompt_injection")]
    PromptInjection,
    
    [EnumMember(Value = "proto_pollution")]
    JsPrototypePollution,
    
    [EnumMember(Value = "rfi")]
    RemoteFileInclusion,
    
    [EnumMember(Value = "sqli")]
    SqlInjection,
    
    [EnumMember(Value = "secret_tokens")]
    SecretTokensLeak,
    
    [EnumMember(Value = "server_side_js_injection")]
    ServerSideJsInjection,
    
    [EnumMember(Value = "ssrf")]
    ServerSideRequestForgery,
    
    [EnumMember(Value = "ssti")]
    ServerSideTemplateInjection,
    
    [EnumMember(Value = "stored_xss")]
    StoredCrossSiteScripting,
    
    [EnumMember(Value = "unvalidated_redirect")]
    UnvalidatedRedirect,
    
    [EnumMember(Value = "xpathi")]
    XpathInjection,
    
    [EnumMember(Value = "xxe")]
    XmlExternalEntityInjection,
    
    [EnumMember(Value = "xss")]
    CrossSiteScripting
}