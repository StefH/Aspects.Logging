using System.Diagnostics;
using Aspects.Logging.DependencyInjection;
using Aspects.Logging.Enums;
using Aspects.Logging.Events;
using Aspects.Logging.Options;
using Aspects.Logging.Utils;
using Aspects.Universal.Attributes;
using Aspects.Universal.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aspects.Logging.Attributes;

public abstract class AbstractLogAspectAttribute : BaseUniversalWrapperAttribute
{
    private readonly Lazy<IGuidProvider?> _guidProvider = AspectServiceLocator.GetService<IGuidProvider>();
    private readonly Lazy<AspectLoggingOptions?> _options = new(() => AspectServiceLocator.GetService<IOptions<AspectLoggingOptions>>().Value?.Value);

    protected const LogLevel DefaultLogLevel = LogLevel.Debug;
    protected const LogPoint DefaultLogPoint = LogPoint.BeforeAndAfterAndFinally;
    protected const LogOptions DefaultLogOptions = LogOptions.Guid;

    protected LogLevel? AspectLogLevel { get; }

    protected LogPoint? AspectLogPoint { get; }

    protected LogOptions? AspectLogOptions { get; }

    /// <summary>
    /// The optional prefix to add before each log-statement.
    /// </summary>
    protected string? Prefix { get; }

    protected AbstractLogAspectAttribute()
    {
    }

    protected AbstractLogAspectAttribute(string prefix)
    {
        Prefix = prefix;
    }

    protected AbstractLogAspectAttribute(LogOptions logOptions = DefaultLogOptions, string? prefix = null)
    {
        AspectLogOptions = logOptions;
        Prefix = prefix;
    }

    protected AbstractLogAspectAttribute(LogPoint logPoint = DefaultLogPoint, LogOptions logOptions = DefaultLogOptions, string? prefix = null)
    {
        AspectLogPoint = logPoint;
        AspectLogOptions = logOptions;
        Prefix = prefix;
    }

    protected AbstractLogAspectAttribute(LogLevel logLevel, LogPoint logPoint = DefaultLogPoint, LogOptions logOptions = DefaultLogOptions, string? prefix = null)
    {
        AspectLogLevel = logLevel;
        AspectLogPoint = logPoint;
        AspectLogOptions = logOptions;
        Prefix = prefix;
    }

    protected sealed override T WrapSync<T>(Func<object[], T> target, object[] args, AspectEventArgs eventArgs)
    {
        var loggingEventArgs = CreateLoggingAspectEventArgs(eventArgs);
        var stopwatch = Stopwatch.StartNew();
        var logPoint = GetLogPoint().Value;

        if (logPoint.HasFlag(LogPoint.Before))
        {
            OnBefore(loggingEventArgs);
        }

        try
        {
            var result = base.WrapSync(target, args, eventArgs);

            if (logPoint.HasFlag(LogPoint.After))
            {
                OnAfter(loggingEventArgs);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (logPoint.HasFlag(LogPoint.Exception))
            {
                return OnException<T>(loggingEventArgs, ex);
            }

            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (logPoint.HasFlag(LogPoint.Finally))
            {
                OnFinally(loggingEventArgs, stopwatch.Elapsed);
            }
        }
    }

    protected sealed override async Task<T> WrapAsync<T>(Func<object[], Task<T>> target, object[] args, AspectEventArgs eventArgs)
    {
        var loggingEventArgs = CreateLoggingAspectEventArgs(eventArgs);
        var stopwatch = Stopwatch.StartNew();
        var logPoint = GetLogPoint().Value;

        if (logPoint.HasFlag(LogPoint.Before))
        {
            OnBefore(loggingEventArgs);
        }

        try
        {
            var result = await target(args).ConfigureAwait(false);

            if (logPoint.HasFlag(LogPoint.After))
            {
                OnAfter(loggingEventArgs);
            }

            return result;
        }
        catch (Exception ex)
        {
            if (logPoint.HasFlag(LogPoint.Exception))
            {
                return OnException<T>(loggingEventArgs, ex);
            }

            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (logPoint.HasFlag(LogPoint.Finally))
            {
                OnFinally(loggingEventArgs, stopwatch.Elapsed);
            }
        }
    }

    protected virtual void OnBefore(LoggingAspectEventArgs eventArgs)
    {
    }

    protected virtual void OnAfter(LoggingAspectEventArgs eventArgs)
    {
    }

    protected virtual void OnFinally(LoggingAspectEventArgs eventArgs, TimeSpan elapsed)
    {
    }

    protected virtual T OnException<T>(LoggingAspectEventArgs eventArgs, Exception exception) => throw exception;

    private LoggingAspectEventArgs CreateLoggingAspectEventArgs(AspectEventArgs eventArgs) => new(eventArgs, CreateLoggingGuid(), GetPrefix(), GetLogLevel().Value);

    private string? GetPrefix() => Prefix ?? _options.Value?.Prefix;

    private Guid? CreateLoggingGuid() => GetLogOptions().Value.HasFlag(LogOptions.Guid) ? _guidProvider.Value?.NewGuid() ?? Guid.NewGuid() : null;

    private Lazy<LogLevel> GetLogLevel()
    {
        return new Lazy<LogLevel>(() =>
        {
            // If LogLevel is defined for this attribute, use that.
            if (AspectLogLevel != null)
            {
                return AspectLogLevel.Value;
            }

            // If the LogLevel is set in the options use the LogLevel from options.
            if (_options.Value is { LogLevel: { } })
            {
                return _options.Value.LogLevel.Value;
            }

            // Else use the default (Debug).
            return DefaultLogLevel;
        });
    }

    private Lazy<LogPoint> GetLogPoint()
    {
        return new Lazy<LogPoint>(() =>
        {
            // If LogPoint is defined for this attribute, use that.
            if (AspectLogPoint != null)
            {
                return AspectLogPoint.Value;
            }

            // If the LogPoint is set in the options use the LogPoint from options.
            if (_options.Value is { LogPoint: { } })
            {
                return _options.Value.LogPoint.Value;
            }

            // Else use the default (BeforeAndAfterAndFinally).
            return DefaultLogPoint;
        });
    }

    private Lazy<LogOptions> GetLogOptions()
    {
        return new Lazy<LogOptions>(() =>
        {
            // If LogOptions is defined for this attribute, use that.
            if (AspectLogOptions != null)
            {
                return AspectLogOptions.Value;
            }

            // If the LogOptions is set in the options use the LogOptions from options.
            if (_options.Value is { LogOptions: { } })
            {
                return _options.Value.LogOptions.Value;
            }

            // Else use the default (Guid).
            return DefaultLogOptions;
        });
    }
}