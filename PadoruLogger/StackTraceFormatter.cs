using System.Diagnostics;

namespace Padoru.Diagnostics
{
    public abstract class StackTraceFormatter
    {
        public abstract string GetFormattedStackTrace(StackTrace stacktrace);
    }
}
