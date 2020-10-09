namespace Padoru.Diagnostics
{
    [System.Serializable]
    public class LogSettings
    {
        public LogType StacktraceLogType { get; set; } = LogType.Error;
        public LogType LogType { get; set; } = LogType.Error;
    }
}
