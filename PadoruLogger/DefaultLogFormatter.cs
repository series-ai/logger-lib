using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Padoru.Diagnostics
{
    public class DefaultLogFormatter : LogFormatter
    {
        private StringBuilder sb;

        public DefaultLogFormatter()
        {
            sb = new StringBuilder();
        }

        public override string GetFormattedLog(LogData logData)
        {
            sb.Clear();

            sb.Append(@"<b>");
            sb.Append(" [");
            sb.Append(logData.dateTime);
            sb.Append("]");
            sb.Append("[");
            sb.Append(logData.contextClass);
            sb.Append(".");
            sb.Append(logData.contextMethod);
            sb.Append("]");
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
                sb.Append(logData.message == null ? "NULL" : logData.message);
            }

            return sb.ToString();
        }
    }
}
