using System.Collections.Generic;
using UnityEngine;

namespace Padoru.Diagnostics
{
    [System.Serializable]
    public class LogSettings
    {
        public LogType StacktraceLogType = LogType.Error;
        public LogType LogType = LogType.Error;
        public List<RuntimePlatform> UnsupportedPlatforms;
    }
}
