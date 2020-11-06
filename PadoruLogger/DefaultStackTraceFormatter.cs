using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Padoru.Diagnostics
{
    public class DefaultStackTraceFormatter : StackTraceFormatter
    {
        private StringBuilder sb;

        public DefaultStackTraceFormatter()
        {
            sb = new StringBuilder();
        }

        public override string GetFormattedStackTrace(StackTrace stacktrace)
        {
            sb.Clear();
            for (int i = 0; i < stacktrace.FrameCount; i++)
            {
                if (i == 0) continue;

                StackFrame frame = stacktrace.GetFrame(i);
                sb.Append($"at {frame.GetMethod().ToString().Split(' ')[1]}");
                string filename = frame.GetFileName();

                if (!string.IsNullOrWhiteSpace(filename))
                {
                    string[] pathElements = filename.Split('\\');

                    int assetDirPosition = 0;

                    for (int j = 0; j < pathElements.Length; j++)
                    {
                        string element = pathElements[j];
                        if (element.Contains("Assets"))
                        {
                            assetDirPosition = j;
                        }
                    }

                    StringBuilder shortPath = new StringBuilder();
                    var index = 0;
                    try
                    {
                        shortPath.Append('\\');
                        for (int k = assetDirPosition + 1; k < pathElements.Length; k++)
                        {
                            index = k;
                            shortPath.Append(pathElements[k]);
                            if (k < pathElements.Length - 1)
                            {
                                shortPath.Append('\\');
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        sb.Append($"Error trying to access pathElements[{index}]: {e.Message}");
                    }

                    sb.Append($" (in: {shortPath.ToString()}:{frame.GetFileLineNumber()})");

                }
                else
                {
                    // Cannot get filename because it is probably inside an assembly
                    sb.Append(" (N/A)");
                }

                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
