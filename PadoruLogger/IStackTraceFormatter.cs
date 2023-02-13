using System.Diagnostics;

namespace Padoru.Diagnostics
{
    public interface IStackTraceFormatter
    {
       string GetFormattedStackTrace(StackTrace stacktrace);
    }
}
