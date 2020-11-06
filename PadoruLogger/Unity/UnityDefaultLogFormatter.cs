using System.Text;

namespace Padoru.Diagnostics
{
    public class UnityDefaultLogFormatter : LogFormatter
    {
        private StringBuilder sb;

        public UnityDefaultLogFormatter()
        {
            sb = new StringBuilder();
        }

        public override string GetFormattedLog(LogData logData)
        {
            sb.Clear();

            sb.Append(@"<b>");
            sb.Append("[");
            sb.Append(logData.contextClass);
            sb.Append(".");
            sb.Append(logData.contextMethod);
            sb.Append("]");
            sb.Append(@"</b>");
            sb.Append(" [");
            sb.Append(logData.logType);
            sb.Append("]");

            if (!string.IsNullOrWhiteSpace(logData.message.ToString()))
            {
                sb.Append(": ");
                sb.Append(logData.message == null ? "NULL" : logData.message);
            }

            return sb.ToString();
        }
    }
}
