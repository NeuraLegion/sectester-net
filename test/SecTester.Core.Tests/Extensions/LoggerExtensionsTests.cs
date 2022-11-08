using SecTester.Core.Extensions;
using SecTester.Core.Logger;

namespace SecTester.Core.Tests.Extensions;

public class LoggerExtensionsTests
{
  [Fact]
  public void Error_GivenExceptionInstance_LogErrorWithStringValue()
  {
    // arrange
    var loggerMock = Substitute.For<ILogger>();
    var exception = new Exception("message");
    
    // act
    loggerMock.Error(exception);

    // assert
    loggerMock.Received(1).Error(exception.ToString());
  }
}
