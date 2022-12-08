namespace SecTester.Scan.Tests.CI;

public class ResourceUtilsTests
{
  [Fact]
  public void GetEmbeddedResourceContent_GivenEmptyResourceName_ThrowsError()
  {
    // act
    var act = () => ResourceUtils.GetEmbeddedResourceContent<ResourceUtilsTests>("");

    // assert
    act.Should().Throw<ArgumentNullException>().WithMessage("*resourceName*");
  }

  [Fact]
  public void GetEmbeddedResourceContent_GivenNonExistingResourceName_ThrowsError()
  {
    // act
    var act = () => ResourceUtils.GetEmbeddedResourceContent<ResourceUtilsTests>("ResourceName");

    // assert
    act.Should().Throw<InvalidOperationException>().WithMessage($"Could not get stream for ResourceName resource.");
  }

  [Fact]
  public void GetEmbeddedResourceContent_GivenExistingResourceName_ReturnsContent()
  {
    // act
    var act = () => ResourceUtils.GetEmbeddedResourceContent<DefaultCiDiscovery>("SecTester.Scan.CI.vendors.json");

    // assert
    act.Should().NotBeNull();
  }
}
