namespace SecTester.Repeater.Tests.Fixtures;

internal class TestServerApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
  where TStartup : class
{
  protected override TestServer CreateServer(IWebHostBuilder builder) =>
    base.CreateServer(
      builder.UseSolutionRelativeContentRoot(""));

  protected override IWebHostBuilder CreateWebHostBuilder() =>
    WebHost.CreateDefaultBuilder()
      .UseStartup<TStartup>();



}
