using System.Text;

namespace Padoru.Diagnostics
{
    public class UnityDefaultLogFormatter : ILogFormatter
    {
        private readonly StringBuilder sb;

        public UnityDefaultLogFormatter()
        {
            sb = new StringBuilder();
        }

        public string GetFormattedLog(LogData logData)
        {
            sb.Clear();

            sb.Append(@"<b>");
            
            // This might happen on platforms where we can't get stack traces
            if(!string.IsNullOrWhiteSpace(logData.contextClass) && !string.IsNullOrWhiteSpace(logData.contextMethod))
            {
                sb.Append("[");
                sb.Append(logData.contextClass);
                sb.Append(".");
                sb.Append(logData.contextMethod);
                sb.Append("]");
            }
            
            sb.Append(@"</b>");
            sb.Append(" [");
            sb.Append(logData.logType);
            sb.Append("]");
            sb.Append(" [");
            sb.Append(logData.channel);
            sb.Append("]");

            if (!string.IsNullOrWhiteSpace(logData.message.ToString()))
            {
                sb.Append(": ");
                sb.Append(logData.message ?? "NULL");
            }

            return sb.ToString();
        }
    }
}
