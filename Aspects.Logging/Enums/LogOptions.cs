namespace Aspects.Logging.Enums;

[Flags]
public enum LogOptions
{
    /// <summary>
    /// Do not add extra data in the log-statement.
    /// </summary>
    None = 0b00,

    /// <summary>
    /// Add a LoggingGuid in all log-statements (default).
    /// </summary>
    Guid = 0b01
}