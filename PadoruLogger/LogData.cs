using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
    }
}
