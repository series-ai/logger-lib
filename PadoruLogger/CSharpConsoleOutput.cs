using System;
using System.Collections.Generic;
using System.Text;

namespace Padoru.Diagnostics
{
    class CSharpConsoleOutput : IDebugOutput
    {
        public void WriteToOuput(LogType logType, object message, string channel, object context)
        {
            Console.WriteLine(message);
        }
    }
}
