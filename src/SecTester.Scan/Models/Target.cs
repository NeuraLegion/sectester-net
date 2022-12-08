using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SecTester.Core.Utils;
using SecTester.Scan.Models.HarSpec;

namespace SecTester.Scan.Models;

public sealed class Target : TargetOptions
{
  private const string ContentTypeHeaderField = "content-type";
  private const string SetCookieHeaderField = "set-cookie";
  private const string VersionHeaderField = "version";

  private const string TextPlainMimeType = "text/plain";
  private const string MultipartFormDataMimeType = "multipart/form-data";
  private const string FormUrlEncodedMimeType = "application/x-www-form-urlencoded";

  private const string DefaultHttpVersion = "HTTP/0.9";

  private readonly Dictionary<string, IEnumerable<string>> _computedHeaders = new();
  private readonly Dictionary<string, string?> _computedHeaderValues = new();

  private Func<Task<PostData>>? _bodyGenerator;
  private string? _computedQuery;

  /// <summary>
  ///   Create a target with server URL that will be used for the request
  /// </summary>
  public Target(string url) : this(new Uri(url))
  {
  }

  /// <summary>
  ///   Create a target with server URL that will be used for the request
  /// </summary>
  public Target(Uri uri)
  {
    var url = uri.ToString();
    _computedQuery ??= uri.Query;
    Url = UrlUtils.NormalizeUrl(url);
  }

  internal Target(TargetOptions options) : this(options.Url)
  {
    if (options.Query is not null)
    {
      WithQuery(options.Query, options.SerializeQuery);
    }

    if (options.Headers is not null)
    {
      WithHeaders(options.Headers);
    }

    if (options.Body is not null)
    {
      WithBody(options.Body);
    }

    if (options.Method is not null)
    {
      WithMethod(options.Method);
    }
  }

  public string? ContentType => GetHeaderValue(ContentTypeHeaderField);

  public string HttpVersion
  {
    get
    {
      var version = GetHeaderValue(VersionHeaderField) ?? GetHeaderValue($":{VersionHeaderField}");

      return !string.IsNullOrEmpty(version) ? version! : DefaultHttpVersion;
    }
  }

  public string Url { get; }

  public IEnumerable<KeyValuePair<string, string>>? Query { get; private set; }

  public HttpContent? Body { get; private set; }
  public HttpMethod Method { get; private set; } = HttpMethod.Get;

  public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? Headers { get; private set; }

  public string SerializeQuery(IEnumerable<KeyValuePair<string, string>> pairs)
  {
    return UrlUtils.SerializeQuery(pairs);
  }

  /// <summary>
  ///   Sets query parameters to be sent with the request
  /// </summary>
  public Target WithQuery(IEnumerable<KeyValuePair<string, string>> value)
  {
    return WithQuery(value, SerializeQuery);
  }

  /// <summary>
  ///   Sets query parameters to be sent with the request.
  ///   You can pass a function of serializing `Query` providing a custom rules.
  /// </summary>
  public Target WithQuery(IEnumerable<KeyValuePair<string, string>> value,
    Func<IEnumerable<KeyValuePair<string, string>>, string> serializer)
  {
    Query = value ?? throw new ArgumentNullException(nameof(value));
    _computedQuery = serializer(Query);
    return this;
  }


  /// <summary>
  ///   Sets a application/x-www-form-urlencoded to be sent as the request body.
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  public Target WithBody(FormUrlEncodedContent value)
  {
    return WithBody(value, async () =>
    {
      var text = await value.ReadAsStringAsync().ConfigureAwait(true);
      var query = UrlUtils.ParseQuery(text);
      var parameters = query.Select(k => new PostDataParameter(k.Key, k.Value)).ToList();

      return new PostData
      {
        Text = text,
        Params = parameters,
        MimeType = ContentType ?? FormUrlEncodedMimeType
      };
    });
  }

  /// <summary>
  ///   Sets a multipart/form-data to be sent as the request body.
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  public Target WithBody(MultipartContent value)
  {
    return WithBody(value, async () =>
    {
      var text = await value.ReadAsStringAsync().ConfigureAwait(true);
      var tasks = value.Select(async k =>
      {
        var contentDisposition = k.Headers.ContentDisposition;
        var mediaTypeHeaderValue = k.Headers.ContentType;
        var content = await k.ReadAsStringAsync().ConfigureAwait(false);

        return new PostDataParameter(contentDisposition.Name, content)
        {
          FileName = contentDisposition.FileName,
          ContentType = mediaTypeHeaderValue?.MediaType
        };
      });
      var parameters = await Task.WhenAll(tasks).ConfigureAwait(false);

      return new PostData
      {
        Text = text,
        Params = parameters,
        MimeType = ContentType ?? MultipartFormDataMimeType
      };
    });
  }

  /// <summary>
  ///   Sets a data to be sent as the request body.
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  public Target WithBody(HttpContent value)
  {
    return WithBody(value, async () =>
    {
      var text = await value.ReadAsStringAsync().ConfigureAwait(true);

      return new PostData
      {
        Text = text,
        MimeType = ContentType ?? TextPlainMimeType
      };
    });
  }

  /// <summary>
  ///   Sets a string to be sent as the request body with a concrete mime type
  ///   The only required for POST, PUT, PATCH, and DELETE
  /// </summary>
  public Target WithBody(string value, string contentType = TextPlainMimeType)
  {
    var content = new StringContent(value, Encoding.Default, contentType);

    return WithBody(content, () => Task.FromResult(new PostData
    {
      Text = value,
      MimeType = ContentType ?? TextPlainMimeType
    }));
  }

  private Target WithBody(HttpContent body, Func<Task<PostData>> buildAction)
  {
    Body = body ?? throw new ArgumentNullException(nameof(body));
    _bodyGenerator = buildAction ?? throw new ArgumentNullException(nameof(buildAction));
    return this;
  }

  /// <summary>
  ///   Set a request method to be used when making the request, GET by default
  /// </summary>
  public Target WithMethod(HttpMethod value)
  {
    Method = value ?? throw new ArgumentNullException(nameof(value));
    return this;
  }

  /// <summary>
  ///   Set a request method to be used when making the request, GET by default
  /// </summary>
  public Target WithMethod(string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      throw new ArgumentNullException(nameof(value));
    }

    var httpMethod = new HttpMethod(value.ToUpperInvariant());
    return WithMethod(httpMethod);
  }

  /// <summary>
  ///   Set headers
  /// </summary>
  public Target WithHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> value)
  {
    Headers = value ?? throw new ArgumentNullException(nameof(value));
    return this;
  }

  /// <summary>
  ///   Returns a HAR request containing detailed info about performed request
  /// </summary>
  internal async Task<RequestMessage> ToHarRequest()
  {
    var uriBuilder = new UriBuilder(Url)
    {
      Query = _computedQuery,
      Fragment = ""
    };
    var url = UrlUtils.NormalizeUrl(uriBuilder.ToString());
    var queryParameters = BuildQueryParameters();
    var headers = BuildHeaderParameters();
    var postData = _bodyGenerator is not null ? await _bodyGenerator.Invoke().ConfigureAwait(false) : null;

    return new RequestMessage
    {
      Url = url,
      Headers = headers,
      PostData = postData,
      HttpVersion = HttpVersion,
      QueryString = queryParameters,
      Method = Method.ToString()
    };
  }

  private IEnumerable<Header> BuildHeaderParameters()
  {
    var headers = Headers?.ToList() ?? new List<KeyValuePair<string, IEnumerable<string>>>();

    if (Body != null)
    {
      headers.AddRange(Body.Headers);
    }

    _computedHeaderValues.Clear();
    _computedHeaders.Clear();

    foreach (var pair in headers)
    {
      _computedHeaders[pair.Key.ToLowerInvariant()] = pair.Value;
    }

    return _computedHeaders.SelectMany(pair =>
      pair.Value.Select(value => new Header(pair.Key, value))).ToList();
  }

  private IEnumerable<QueryParameter> BuildQueryParameters()
  {
    if (string.IsNullOrEmpty(_computedQuery))
    {
      return Array.Empty<QueryParameter>();
    }

    var query = Query ?? UrlUtils.ParseQuery(_computedQuery!);
    return query.Select(k => new QueryParameter(k.Key, k.Value)).ToList();
  }

  private string? GetHeaderValue(string headerField)
  {
    if (!_computedHeaderValues.ContainsKey(headerField))
    {
      _computedHeaderValues.Add(headerField, ComputeHeaderValue(headerField));
    }

    return _computedHeaderValues[headerField];
  }

  private string? ComputeHeaderValue(string headerField)
  {
    if (!_computedHeaders.Any())
    {
      return null;
    }

    var values = _computedHeaders
      .Where(header => header.Key.Equals(headerField, StringComparison.OrdinalIgnoreCase))
      .SelectMany(header => header.Value).ToArray();

    return !values.Any()
      ? null
      : string.Join(headerField.Equals(SetCookieHeaderField, StringComparison.OrdinalIgnoreCase) ? "\n" : ", ", values);

  }
}
