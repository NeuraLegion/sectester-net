using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SecTester.Core.Utils;

public static class UrlUtils
{
  private static readonly Regex LeadingQuestionMarkRegexp = new(@"^\?");
  private static readonly Regex TrailingSlashRegexp = new(@"\/$");
  private static readonly Regex RelativeProtocolRegexp = new(@"^\/{2}");
  private static readonly Regex RelativeUrlRegexp = new(@"^\.+\/");
  private static readonly Regex ProtocolRegexp = new(@"^(?!(?:\w+:)?\/\/)|^\/\/");
  private static readonly Regex EmptySegmentsRegexp = new(@"((?!:).|^)\/{2,}");
  private static readonly Regex NonRootPathRegexp = new(@"^(?!\/)");

  public static string NormalizeUrl(string value)
  {
    var url = new Uri(CleanUpProtocol(value));
    var urlBuilder = new UriBuilder(url);

    urlBuilder.Path = NormalizePathName(urlBuilder.Path);
    urlBuilder.Query = SerializeQuery(ParseQuery(urlBuilder.Query), true);

    if (url.IsDefaultPort)
    {
      urlBuilder.Port = -1;
    }

    var urlString = urlBuilder.ToString();

    if (urlBuilder.Path == "/" && string.IsNullOrEmpty(urlBuilder.Fragment))
    {
      urlString = TrailingSlashRegexp.Replace(urlString, "");
    }

    return urlString;
  }

  public static IEnumerable<KeyValuePair<string, string>> ParseQuery(string queryString)
  {
    var query = HttpUtility.ParseQueryString(queryString);
    return query.AllKeys.ToDictionary(key => key, key => query.Get(key));
  }

  public static string SerializeQuery(IEnumerable<KeyValuePair<string, string>> @params, bool sort = false)
  {
    var pairs = sort ? @params.OrderBy(pair => pair.Key) : @params;
    var builder = new StringBuilder();

    foreach (var pair in pairs)
    {
      if (builder.Length > 0)
      {
        builder.Append('&');
      }

      builder.Append(EncodeUrlFragment(pair.Key));
      builder.Append('=');
      builder.Append(EncodeUrlFragment(pair.Value));
    }

    return LeadingQuestionMarkRegexp.Replace(builder.ToString(), "");
  }

  private static string NormalizePathName(string pathname)
  {
    var result = EmptySegmentsRegexp.Replace(pathname, p =>
    {
      var value = p.Groups[1].Value ?? "";
      return NonRootPathRegexp.IsMatch(value) ? $"{value}/" : "/";
    });

    return result;
  }

  private static string CleanUpProtocol(string url)
  {
    var relativeUrl = !RelativeProtocolRegexp.IsMatch(url) && RelativeUrlRegexp.IsMatch(url);

    if (!relativeUrl)
    {
      url = ProtocolRegexp.Replace(url, $"{Uri.UriSchemeHttps}://");
    }

    return url;
  }

  private static string EncodeUrlFragment(string value)
  {
    return Uri.EscapeDataString(value).Replace("%20", "+");
  }
}
