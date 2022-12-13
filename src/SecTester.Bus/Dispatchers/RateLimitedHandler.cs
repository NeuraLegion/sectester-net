using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace SecTester.Bus.Dispatchers;

[ExcludeFromCodeCoverage]
internal sealed class RateLimitedHandler
  : DelegatingHandler, IAsyncDisposable
{
  private readonly RateLimiter _rateLimiter;

  public RateLimitedHandler(RateLimiter limiter)
    : base(new HttpClientHandler
    {
      AllowAutoRedirect = true,
      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    })
  {
    _rateLimiter = limiter;
  }

  async ValueTask IAsyncDisposable.DisposeAsync()
  {
    await _rateLimiter.DisposeAsync().ConfigureAwait(false);

    Dispose(disposing: false);
    GC.SuppressFinalize(this);
  }

  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, CancellationToken cancellationToken)
  {
    using var lease = await _rateLimiter.AcquireAsync(
      1, cancellationToken).ConfigureAwait(false);

    if (lease.IsAcquired)
    {
      return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    var response = new HttpResponseMessage((HttpStatusCode)429);
    if (lease.TryGetMetadata(
          MetadataName.RetryAfter, out var retryAfter))
    {
      response.Headers.Add(
        "Retry-After",
        ((int)retryAfter.TotalSeconds).ToString(
          NumberFormatInfo.InvariantInfo));
    }

    return response;
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);

    if (disposing)
    {
      _rateLimiter.Dispose();
    }
  }
}
