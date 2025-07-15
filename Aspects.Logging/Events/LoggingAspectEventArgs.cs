using System.Reflection;
using Aspects.Universal.Events;
using Microsoft.Extensions.Logging;

namespace Aspects.Logging.Events;

public class LoggingAspectEventArgs : EventArgs
{
    public IReadOnlyList<object> Args { get; }

    public object? Instance { get; }

    /// <summary>
    /// The LoggingGuid which can be added to all logging statements (can be used to link the OnBefore, OnAfter, OnException and OnFinally calls).
    /// This is optional.
    /// </summary>
    public Guid? LoggingGuid { get; }

    /// <summary>
    /// The LogLevel which is used for each log-statement.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// The prefix to add before each log-statement.
    /// This is optional.
    /// </summary>
    public string? Prefix { get; }

    public MethodBase Method { get; }

    public string Name { get; }

    public Type ReturnType { get; }

    public Attribute[] Triggers { get; }

    public Type Type { get; }

    internal LoggingAspectEventArgs(AspectEventArgs eventArgs, Guid? loggingGuid, string? prefix, LogLevel logLevel)
    {
        Args = eventArgs.Args;
        Instance = eventArgs.Instance;
        LoggingGuid = loggingGuid;
        LogLevel = logLevel;
        Method = eventArgs.Method;
        Name = eventArgs.Name;
        Prefix = prefix;
        ReturnType = eventArgs.ReturnType;
        Triggers = eventArgs.Triggers;
        Type = eventArgs.Type;
    }
}