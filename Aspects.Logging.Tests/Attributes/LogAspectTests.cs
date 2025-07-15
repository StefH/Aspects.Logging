using Aspects.Logging.Attributes;
using Aspects.Logging.DependencyInjection;
using Aspects.Logging.Tests.Examples;
using Aspects.Logging.Utils;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Aspects.Logging.Tests.Attributes;

public class LogAspectTests
{
    private static readonly Guid LoggingGuid = new("74b9abf5-e6c2-4c5c-ae60-87903ac32295");
    private Mock<ILogger<LogAspectAttribute>> _loggerMock = null!;
    private Mock<ILogger<LogPrivateAspectAttribute>> _loggerPrivateMock = null!;
    private Mock<ILogger<LogPublicAspectAttribute>> _loggerPublicMock = null!;

    [SetUp]
    public void Setup()
    {
        var guidProviderMock = new Mock<IGuidProvider>();
        guidProviderMock.Setup(g => g.NewGuid()).Returns(LoggingGuid);

        _loggerMock = new Mock<ILogger<LogAspectAttribute>>();
        _loggerPrivateMock = new Mock<ILogger<LogPrivateAspectAttribute>>();
        _loggerPublicMock = new Mock<ILogger<LogPublicAspectAttribute>>();

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IGuidProvider))).Returns(guidProviderMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogAspectAttribute>))).Returns(_loggerMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogPrivateAspectAttribute>))).Returns(_loggerPrivateMock.Object);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<LogPublicAspectAttribute>))).Returns(_loggerPublicMock.Object);

        serviceProviderMock.Object.InitializeAspectServiceLocator();
    }

    [Test]
    public void TestLogAspectOnClass()
    {
        // Arrange
        var sut = new TestLogAspectOnClass();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspectOnClassWithPrefix()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithPrefix();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestAbc TestLogAspectOnClassWithPrefix.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TestLogAspectOnClassWithLogLevelInformation()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithLogLevelInformation();

        // Act
        _ = await sut.GetAnswerAsync().ConfigureAwait(false);

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation.GetAnswerAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation.GetAnswerAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerMock.VerifyLogging(LogLevel.Information, "TestLogAspectOnClassWithLogLevelInformation.GetAnswerAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspectOnClassLogPointFinally()
    {
        // Arrange
        var sut = new TestLogAspectOnClassLogPointFinally();

        // Act
        _ = sut.GetAnswer();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassLogPointFinally.GetAnswer:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspectOnClassLogPointFinallyWithLogOptionsNone()
    {
        // Arrange
        var sut = new TestLogAspectOnClassLogPointFinallyWithLogOptionsNone();

        // Act
        _ = sut.X();

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassLogPointFinallyWithLogOptionsNone.X:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestLogAspectOnClass_ThrowException()
    {
        // Arrange
        var sut = new TestLogAspectOnClass();

        // Act
        try
        {
            sut.Throw();
        }
        catch (Exception ex)
        {
            ex.Should().BeAssignableTo<NotImplementedException>();
        }

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.Throw:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Error, "TestLogAspectOnClass.Throw:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnException");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.Throw:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TestLogAspectOnClass_Async_ThrowException()
    {
        // Arrange
        var sut = new TestLogAspectOnClass();

        // Act
        try
        {
            await sut.ThrowAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ex.Should().BeAssignableTo<NotImplementedException>();
        }

        // Verify
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.ThrowAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerMock.VerifyLogging(LogLevel.Error, "TestLogAspectOnClass.ThrowAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnException");
        _loggerMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClass.ThrowAsync:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestPublicMethodAnnotatedWithLogPrivate()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithVisibility();

        // Act
        _ = sut.PublicMethodAnnotatedWithLogPrivate();

        // Verify
        _loggerPublicMock.VerifyNoOtherCalls();
        _loggerPrivateMock.VerifyNoOtherCalls();
    }

    [Test]
    public void TestPublicMethodAnnotatedWithLogPublic()
    {
        // Arrange
        var sut = new TestLogAspectOnClassWithVisibility();

        // Act
        sut.PublicMethodAnnotatedWithLogPublic();

        // Verify
        _loggerPrivateMock.VerifyNoOtherCalls();

        _loggerPublicMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithVisibility.PublicMethodAnnotatedWithLogPublic:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnBefore");
        _loggerPublicMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithVisibility.PublicMethodAnnotatedWithLogPublic:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnAfter");
        _loggerPublicMock.VerifyLogging(LogLevel.Debug, "TestLogAspectOnClassWithVisibility.PublicMethodAnnotatedWithLogPublic:74b9abf5-e6c2-4c5c-ae60-87903ac32295:OnFinally (*)");
        _loggerPublicMock.VerifyNoOtherCalls();
    }
}