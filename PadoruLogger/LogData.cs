using System;
using System.Diagnostics;

namespace Padoru.Diagnostics
{
    public class LogData
    {
        public object message;
        public StackTrace stackTrace;
        public string channel;
        public string contextClass;
        public string contextMethod;
        public DateTime dateTime;
        public LogType logType;
        public object context;
    }
}
