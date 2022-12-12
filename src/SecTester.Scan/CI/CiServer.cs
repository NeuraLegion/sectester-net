using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SecTester.Scan.CI;

[DebuggerDisplay("{Id},{Name}")]
public sealed class CiServer : IEquatable<CiServer>
{
  public static CiServer AppCenter { get; } = new("APPCENTER", "Visual Studio App Center");
  public static CiServer AppCircle { get; } = new("APPCIRCLE", "Appcircle");
  public static CiServer Appveyor { get; } = new("APPVEYOR", "AppVeyor");
  public static CiServer AzurePipelines { get; } = new("AZURE_PIPELINES", "Azure Pipelines");
  public static CiServer Bamboo { get; } = new("BAMBOO", "Bamboo");
  public static CiServer Bitbucket { get; } = new("BITBUCKET", "Bitbucket Pipelines");
  public static CiServer Bitrise { get; } = new("BITRISE", "Bitrise");
  public static CiServer Buddy { get; } = new("BUDDY", "Buddy");
  public static CiServer BuildKite { get; } = new("BUILDKITE", "Buildkite");
  public static CiServer Circle { get; } = new("CIRCLE", "CircleCI");
  public static CiServer Cirrus { get; } = new("CIRRUS", "Cirrus CI");
  public static CiServer CodeBuild { get; } = new("CODEBUILD", "AWS CodeBuild");
  public static CiServer CodeFresh { get; } = new("CODEFRESH", "Codefresh");
  public static CiServer CodeMagic { get; } = new("CODEMAGIC", "Codemagic");
  public static CiServer CodeShip { get; } = new("CODESHIP", "Codeship");
  public static CiServer Drone { get; } = new("DRONE", "Drone");
  public static CiServer Dsari { get; } = new("DSARI", "dsari");
  public static CiServer Eas { get; } = new("EAS", "Expo Application Services");
  public static CiServer Gerrit { get; } = new("GERRIT", "Gerrit");
  public static CiServer GithubActions { get; } = new("GITHUB_ACTIONS", "GitHub Actions");
  public static CiServer GitLab { get; } = new("GITLAB", "GitLab CI");
  public static CiServer GoCd { get; } = new("GOCD", "GoCD");
  public static CiServer GoogleCloudBuild { get; } = new("GOOGLE_CLOUD_BUILD", "Google Cloud Build");
  public static CiServer Heroku { get; } = new("HEROKU", "Heroku");
  public static CiServer Hudson { get; } = new("HUDSON", "Hudson");
  public static CiServer Jenkins { get; } = new("JENKINS", "Jenkins");
  public static CiServer LayerCi { get; } = new("LAYERCI", "LayerCI");
  public static CiServer Magnum { get; } = new("MAGNUM", "Magnum CI");
  public static CiServer Netlify { get; } = new("NETLIFY", "Netlify CI");
  public static CiServer NeverCode { get; } = new("NEVERCODE", "Nevercode");
  public static CiServer ReleaseHub { get; } = new("RELEASEHUB", "ReleaseHub");
  public static CiServer Render { get; } = new("RENDER", "Render");
  public static CiServer Sail { get; } = new("SAIL", "Sail CI");
  public static CiServer Screwdriver { get; } = new("SCREWDRIVER", "Screwdriver");
  public static CiServer Semaphore { get; } = new("SEMAPHORE", "Semaphore");
  public static CiServer Shippable { get; } = new("SHIPPABLE", "Shippable");
  public static CiServer Solano { get; } = new("SOLANO", "Solano CI");
  public static CiServer SourceHut { get; } = new("SOURCEHUT", "Sourcehut");
  public static CiServer Strider { get; } = new("STRIDER", "Strider CD");
  public static CiServer TaskCluster { get; } = new("TASKCLUSTER", "TaskCluster");
  public static CiServer TeamCity { get; } = new("TEAMCITY", "TeamCity");
  public static CiServer Travis { get; } = new("TRAVIS", "Travis CI");
  public static CiServer Vercel { get; } = new("VERCEL", "Vercel");
  public static CiServer Woodpecker { get; } = new("WOODPECKER", "Woodpecker");
  public static CiServer XcodeCloud { get; } = new("XCODE_CLOUD", "Xcode Cloud");
  public static CiServer XcodeServer { get; } = new("XCODE_SERVER", "Xcode Server");

  private static readonly IEnumerable<CiServer> All = typeof(CiServer)
    .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
    .Select(x => x.GetValue(null))
    .Cast<CiServer>();

  private readonly int _hashcode;

  public string Id { get; }
  public string Name { get; }

  public CiServer(string id, string name)
  {
    if (string.IsNullOrEmpty(id))
    {
      throw new ArgumentException("Id must not be empty.");
    }

    Id = id;
    Name = name;

    _hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode(Id);
  }

  public override int GetHashCode() => _hashcode;

  public override string ToString() => Name;

  public override bool Equals(object? obj)
  {
    return Equals(obj as CiServer);
  }

  public bool Equals(CiServer? other)
  {
    return other is not null && Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
  }

  public static IEnumerable<CiServer> GetAll() => All;

  public static bool operator ==(CiServer? left, CiServer? right)
  {
    return left is null || right is null ? ReferenceEquals(left, right) : left.Equals(right);
  }

  public static bool operator !=(CiServer? left, CiServer? right)
  {
    return !(left == right);
  }
}
