using AspectInjector.Broker;
using Aspects.Logging.DependencyInjection;
using Aspects.Logging.Enums;
using Aspects.Logging.Events;
using Aspects.Logging.Extensions;
using Aspects.Logging.Models;
using Aspects.Universal.Aspects;
using Microsoft.Extensions.Logging;

namespace Aspects.Logging.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[Injection(typeof(MethodWrapperAspect), Inherited = true)]
public class LogAspectAttribute : AbstractLogAspectAttribute
{
    private readonly Lazy<ILogger<LogAspectAttribute>?> _logger = AspectServiceLocator.GetService<ILogger<LogAspectAttribute>>();

    public LogAspectAttribute()
    {
    }

    public LogAspectAttribute(string prefix) : base(prefix)
    {
    }

    public LogAspectAttribute(LogOptions logOptions = DefaultLogOptions, string? prefix = null) : base(logOptions, prefix)
    {
    }

    public LogAspectAttribute(LogPoint logPoint = DefaultLogPoint, LogOptions logOptions = DefaultLogOptions, string? prefix = null) : base(logPoint, logOptions, prefix)
    {
    }

    public LogAspectAttribute(LogLevel logLevel, LogPoint logPoint = DefaultLogPoint, LogOptions logOptions = DefaultLogOptions, string? prefix = null) : base(logLevel, logPoint, logOptions, prefix)
    {
    }

    protected override void OnBefore(LoggingAspectEventArgs eventArgs)
    {
        _logger.Value.OnBefore(new LogArguments(eventArgs.LogLevel, eventArgs));
    }

    protected override void OnAfter(LoggingAspectEventArgs eventArgs)
    {
        _logger.Value.OnAfter(new LogArguments(eventArgs.LogLevel, eventArgs));
    }

    protected override void OnFinally(LoggingAspectEventArgs eventArgs, TimeSpan elapsed)
    {
        _logger.Value.OnFinally(new LogArguments(eventArgs.LogLevel, eventArgs), elapsed);
    }

    protected override T OnException<T>(LoggingAspectEventArgs eventArgs, Exception exception)
    {
        _logger.Value.OnException(new LogArguments(LogLevel.Error, eventArgs), exception);
        return base.OnException<T>(eventArgs, exception);
    }
}