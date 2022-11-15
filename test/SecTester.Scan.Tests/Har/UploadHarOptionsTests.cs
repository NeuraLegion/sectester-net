#pragma warning disable CS8604
using System.Text.Json;
using SecTester.Scan.Har;

namespace SecTester.Scan.Tests.Har;

public class UploadHarContentOptionsTests
{
  private const string FileName = "filename";
  private static readonly HarContent HarContent = new();

  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    IncludeFields = true,
    Converters = { new JsonStringEnumMemberConverter(JsonNamingPolicy.CamelCase, false) }
  };

  [Fact]
  public void Constructor_WithAllParameters_AssignProperties()
  {
    // act
    var options = new UploadHarOptions(HarContent, FileName, true);

    // assert
    options.HarContent.Should().Be(HarContent);
    options.FileName.Should().Be(FileName);
    options.Discard.Should().Be(true);
  }

  [Fact]
  public void Constructor_GivenNullHarContent_ThrowError()
  {
    // act
    Action act = () => new UploadHarOptions(null as HarContent, FileName);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_GivenNullFileName_ThrowError()
  {
    // act
    Action act = () => new UploadHarOptions(HarContent, null);

    // assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void FileName_DeserializeLowercaseFilename_Deserialized()
  {
    // arrange
    var payload = @"{""harContent"":{}, ""filename"":""filename""}";

    // act
    var options = JsonSerializer.Deserialize<UploadHarOptions>(payload, JsonSerializerOptions);

    // assert
    options.Should().NotBeNull();
    options?.FileName.Should().Be("filename");
  }

  [Fact]
  public void FileName_SerializeLowercaseFilename_Serialized()
  {
    // arrange
    var options = new UploadHarOptions(HarContent, FileName, true);

    // act
    var data = JsonSerializer.Serialize(options, JsonSerializerOptions);

    // assert
    data.Should().Contain(@"""filename"":""filename""");
  }
}
