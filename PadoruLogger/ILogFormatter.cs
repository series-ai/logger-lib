namespace Padoru.Diagnostics
{
    public interface ILogFormatter
    {
        string GetFormattedLog(LogData logData);
    }
}
