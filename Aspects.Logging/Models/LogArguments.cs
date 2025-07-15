using Aspects.Logging.Events;
using Microsoft.Extensions.Logging;

namespace Aspects.Logging.Models;

internal record LogArguments(LogLevel LogLevel, LoggingAspectEventArgs EventArgs);