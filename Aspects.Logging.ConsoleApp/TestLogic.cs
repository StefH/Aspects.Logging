using Aspects.Logging.Attributes;
using Aspects.Logging.Enums;
using Microsoft.Extensions.Logging;
using Stef.Validation;

namespace Aspects.Logging.ConsoleApp;

class TestLogic
{
    private readonly ILogger<TestLogic> _logger;

    public TestLogic(ILogger<TestLogic> logger)
    {
        _logger = Guard.NotNull(logger);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await PrivateVoidAsync(42, cancellationToken);

        // Should not log
        await PublicVoidAsync(42, cancellationToken);

        TestFinally();
        TestFinallyWithoutGuidAndWithDifferentPrefix();
        await TestMultipleAsync();

        var test = new Test();
        test.X();

        try
        {
            test.Throw();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error - Throw");
        }

        try
        {
            await test.ThrowAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error - ThrowAsync");
        }
    }

    [LogPrivateAspect(LogOptions.None)]
    private static async Task PrivateVoidAsync(int x, CancellationToken cancellationToken)
    {
        await Task.Delay(1234, cancellationToken);
    }

    [LogPrivateAspect]
    public static async Task PublicVoidAsync(int x, CancellationToken cancellationToken)
    {
        await Task.Delay(1234, cancellationToken);
    }

    [LogAspect(logPoint: LogPoint.Finally)]
    public void TestFinally()
    {
        Thread.Sleep(100);
    }

    [LogAspect(logPoint: LogPoint.Finally, logOptions: LogOptions.None, prefix: "TestAbc")]
    public void TestFinallyWithoutGuidAndWithDifferentPrefix()
    {
        Thread.Sleep(200);
    }

    [LogAspect]
    public async Task TestMultipleAsync()
    {
        await Task.Delay(500);

        TestFinally();

        await Task.Delay(500);

        TestFinallyWithoutGuidAndWithDifferentPrefix();
    }
}

[LogAspect(LogLevel.Information)]
class Test
{
    public void X()
    {
        Y();
    }

    private void Y()
    {
    }

    public void Throw()
    {
        var t = new TestEx();
        t.Do();
    }

    public Task ThrowAsync()
    {
        throw new AccessViolationException("test async");
    }
}

class TestEx
{
    public void Do()
    {
        throw new AccessViolationException("TestEx");
    }
}