namespace Aspects.Logging.Enums;

[Flags]
public enum LogPoint
{
    None = 0b0000,

    Before = 0b0001,

    After = 0b0010,

    BeforeAndAfter = Before | After,

    Finally = 0b0100,

    BeforeAndAfterAndFinally = Before | After | Finally,

    Exception = 0b1000,

    All = Before | After | Finally | Exception
}