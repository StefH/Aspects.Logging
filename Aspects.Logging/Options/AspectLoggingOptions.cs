using Aspects.Logging.Enums;
using Microsoft.Extensions.Logging;

namespace Aspects.Logging.Options;

public class AspectLoggingOptions
{
    /// <summary>
    /// The optional prefix to add before each log-statement.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// The LogLevel which is used for each log-statement.
    ///
    /// Note that the LogLevel from the attribute overrules this value.
    /// </summary>
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// The LogPoint which is used for each log-statement.
    ///
    /// Note that the LogPoint from the attribute overrules this value.
    /// </summary>
    public LogPoint? LogPoint { get; set; }

    /// <summary>
    /// The LogOptions which is used for each log-statement.
    ///
    /// Note that the LogOptions from the attribute overrules this value.
    /// </summary>
    public LogOptions? LogOptions { get; set; }
}