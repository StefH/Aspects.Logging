using Aspects.Logging.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Aspects.Logging.Extensions;

internal static class LoggerExtensions
{
    public static void OnBefore(this ILogger? logger, LogArguments logArguments)
    {
        logger.LogEvent(logArguments, "{logPoint}", nameof(OnBefore));
    }

    public static void OnAfter(this ILogger? logger, LogArguments logArguments)
    {
        logger.LogEvent(logArguments, "{logPoint}", nameof(OnAfter));
    }

    public static void OnException(this ILogger? logger, LogArguments logArguments, Exception exception)
    {
        var (format, args) = GetFormatWithArgs(logArguments, "{logPoint}", nameof(OnException));

        logger?.LogError(exception, format, args);
    }

    public static void OnFinally(this ILogger? logger, LogArguments args, TimeSpan elapsed)
    {
        logger.LogEvent(args, "{logPoint} ({elapsed})", nameof(OnFinally), elapsed);
    }

    private static void LogEvent(this ILogger? logger, LogArguments logArguments, [StructuredMessageTemplate] string message, params object?[] extraArgs)
    {
        var (format, args) = GetFormatWithArgs(logArguments, message, extraArgs);

        logger?.Log(logArguments.LogLevel, format, args);
    }

    private static (string Format, object?[] Args) GetFormatWithArgs(LogArguments logArguments, [StructuredMessageTemplate] string message, params object?[] extraArgs)
    {
        var formats = new List<string>();
        var args = new List<object?>();

        if (!string.IsNullOrEmpty(logArguments.EventArgs.Prefix))
        {
            formats.Add("{prefix} ");
            args.Add(logArguments.EventArgs.Prefix);
        }

        formats.Add("{name}.");
        args.Add(logArguments.EventArgs.Type.Name);

        formats.Add("{method}:");
        args.Add(logArguments.EventArgs.Name);

        if (logArguments.EventArgs.LoggingGuid != null)
        {
            formats.Add("{guid}:");
            args.Add(logArguments.EventArgs.LoggingGuid);
        }

        formats.Add(message);
        args.AddRange(extraArgs);

        return (string.Concat(formats), args.ToArray());
    }
}