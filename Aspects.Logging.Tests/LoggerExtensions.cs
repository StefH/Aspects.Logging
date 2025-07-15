using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aspects.Logging.Tests;

[ExcludeFromCodeCoverage]
public static class LoggerExtensions
{
    /// <summary>
    /// Based on https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
    /// </summary>
    public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> loggerMock, LogLevel expectedLogLevel = LogLevel.Debug, string? expectedMessage = null, Times? times = null)
    {
        times ??= Times.Once();

        Func<object, Type, bool> state = (v, _) => expectedMessage == null || IsMatch(v?.ToString() ?? string.Empty, expectedMessage);

        loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(l => l == expectedLogLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), (Times)times);

        return loggerMock;
    }

    public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> loggerMock, LogLevel expectedLogLevel = LogLevel.Debug, Times? times = null)
    {
        return loggerMock.VerifyLogging(expectedLogLevel, null, times);
    }

    private static bool IsMatch(string input, string pattern)
    {
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
    }
}