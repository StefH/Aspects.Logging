using Aspects.Logging.Attributes;
using Aspects.Logging.Enums;
using Microsoft.Extensions.Logging;

// For each test scenario, there must be a new class, as the attribute on a class is only initialized once.
namespace Aspects.Logging.Tests.Examples;

[LogAspect(logPoint: LogPoint.All)]
public class TestLogAspectOnClass
{
    public int GetAnswer()
    {
        return 42;
    }

    public void Throw()
    {
        throw new NotImplementedException();
    }

    public Task ThrowAsync()
    {
        throw new NotImplementedException();
    }
}

[LogAspect(prefix: "TestAbc")]
public class TestLogAspectOnClassWithPrefix
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(prefix: "TestAbc")]
public class TestLogAspectOnClassWithPrefix2
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithPrefix3
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(LogLevel.Information)]
public class TestLogAspectOnClassWithLogLevelInformation
{
    public async Task<int> GetAnswerAsync()
    {
        await Task.Delay(100);
        return 42;
    }
}

[LogAspect(LogLevel.Information)]
public class TestLogAspectOnClassWithLogLevelInformation2
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogLevel
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogLevel2
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(logPoint: LogPoint.Finally)]
public class TestLogAspectOnClassLogPointFinally
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(logPoint: LogPoint.Finally, logOptions: LogOptions.None)]
public class TestLogAspectOnClassLogPointFinallyWithLogOptionsNone
{
    public int X()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogPoint
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogPoint2
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(LogPoint.Finally)]
public class TestLogAspectOnClassWithLogPointFinally
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogOptions
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect]
public class TestLogAspectOnClassWithoutLogOptions2
{
    public int GetAnswer()
    {
        return 42;
    }
}

[LogAspect(LogOptions.None)]
public class TestLogAspectOnClassWithLogOptionsNone
{
    public int GetAnswer()
    {
        return 42;
    }
}

public class TestLogAspectOnClassWithVisibility
{
    [LogPrivateAspect]
    public int PublicMethodAnnotatedWithLogPrivate()
    {
        return 42;
    }

    [LogPrivateAspect]
    private int PrivateMethodAnnotatedWithLogPrivate()
    {
        return 42;
    }

    [LogPublicAspect]
    public int PublicMethodAnnotatedWithLogPublic()
    {
        return 42;
    }

    [LogPublicAspect]
    private int PrivateMethodAnnotatedWithLogPublic()
    {
        return 42;
    }
}