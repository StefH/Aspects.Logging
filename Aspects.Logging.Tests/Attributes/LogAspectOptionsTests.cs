using Aspects.Logging.Attributes;
using Aspects.Logging.DependencyInjection;
using Aspects.Logging.Enums;
using Aspects.Logging.Options;
using Aspects.Logging.Tests.Examples;
using Aspects.Logging.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Aspects.Logging.Tests.Attributes;

public class LogAspectOptionsTests
{
    private static readonly Guid LoggingGuid = new("74b9abf5-e6c2-4c5c-ae60-87903ac32295");

    private Mock<IOptions<AspectLoggingOptions>> _optionsMock = null!;
    private Mock<ILogger<LogAspectAttribute>> _loggerMock = null!;
    private Mock<ILogger<LogPrivateAspectAttribute>> _loggerPrivateMock = null!;
    private Mock<ILogger<LogPublicAspectAttribute>> _loggerPublicMock = null!;

    [SetUp]
    public void Setup()
    {
        var guidProviderMock = new Mock<IGuidProvider>();
        guidProviderMock.Setup(g => g.NewGuid()).Returns(LoggingGuid);

        _optionsMock = new Mock<IOptions<AspectLoggingOptions>>();

        var options = new AspectLoggingOptions
        {
            Prefix = "PrefixViaOptions"
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        _loggerMock = new Mock<ILogger<LogAspectAttribute>>();
        _loggerPrivateMock = new Mock<ILogger<LogPrivateAspectAttribute>>();
        _loggerPublicMock = new Mock<ILogger<LogPublicAspectAttribute>>();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IOptions<AspectLoggingOptions>))).Returns(_optionsMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IGuidProvider))).Returns(guidProviderMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogAspectAttribute>))).Returns(_loggerMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogPrivateAspectAttribute>))).Returns(_loggerPrivateMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogPublicAspectAttribute>))).Returns(_loggerPublicMock.Object);

        serviceProviderMock.Object.InitializeAspectServiceLocator();
    }

    [Test]
    public void TestLogAspect_When_PrefixIsDefinedOnAttribute_And_PrefixIsDefinedViaOptions_Should_TakePrefixFromAttribute()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithPrefix2();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_PrefixIsNotDefinedOnAttribute_And_PrefixIsDefinedViaOptions_Should_TakePrefixFromOptions()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithPrefix3();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "PrefixViaOptions TestLogAspectOnClassWithPrefix3.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "PrefixViaOptions TestLogAspectOnClassWithPrefix3.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "PrefixViaOptions TestLogAspectOnClassWithPrefix3.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogLevelIsDefinedViaOptions_And_NoLogLevelIsDefinedOnAttribute_Should_TakeLogLevelFromOptions()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogLevel = LogLevel.Trace
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogLevel();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Trace, "TestLogAspectOnClassWithoutLogLevel.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Trace, "TestLogAspectOnClassWithoutLogLevel.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Trace, "TestLogAspectOnClassWithoutLogLevel.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogLevelIsDefinedViaOptions_And_LogLevelIsDefinedOnAttribute_Should_TakeLogLevelFromAttribute()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogLevel = LogLevel.Critical
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithLogLevelInformation2();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_NoLogLevelIsDefinedViaOptions_And_NoLogLevelIsDefinedOnAttribute_Should_UseDefaultLogLevelDebug()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogLevel = null
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogLevel2();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogLevel2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogLevel2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogLevel2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogPointIsDefinedViaOptions_And_NoLogPointIsDefinedOnAttribute_Should_TakeLogPointFromOptions()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogPoint = LogPoint.BeforeAndAfter
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogPoint();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogPoint.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogPoint.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogPointIsDefinedViaOptions_And_LogPointIsDefinedOnAttribute_Should_TakeLogPointFromAttribute()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogPoint = LogPoint.BeforeAndAfter
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithLogPointFinally();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithLogPointFinally.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_NoLogPointIsDefinedViaOptions_And_NoLogPointIsDefinedOnAttribute_Should_UseDefaultLogPointBeforeAndAfterAndFinally()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogPoint = null
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogPoint2();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogPoint2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogPoint2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogPoint2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogOptionsIsDefinedViaOptions_And_NoLogOptionsIsDefinedOnAttribute_Should_TakeLogOptionsFromOptions()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogOptions = LogOptions.None
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogOptions();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions.GetAnswer:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions.GetAnswer:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions.GetAnswer:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_LogOptionsIsDefinedViaOptions_And_LogOptionsIsDefinedOnAttribute_Should_TakeLogOptionsFromAttribute()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogOptions = LogOptions.Guid
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithLogOptionsNone();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithLogOptionsNone.GetAnswer:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithLogOptionsNone.GetAnswer:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithLogOptionsNone.GetAnswer:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspect_When_NoLogOptionsIsDefinedViaOptions_And_NoLogOptionsIsDefinedOnAttribute_Should_UseDefaultLogOptionsGuid()
    {
        // Arrange
        var options = new AspectLoggingOptions
        {
            LogOptions = null
        };
        _optionsMock.Setup(o => o.Value).Returns(options);

        var sut = new TestLogAspectOnClassWithoutLogOptions2();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithoutLogOptions2.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }
}