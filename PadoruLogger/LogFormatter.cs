using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Padoru.Diagnostics
{
    public abstract class LogFormatter
    {
        public abstract string GetFormattedLog(LogData logData);
    }
}
