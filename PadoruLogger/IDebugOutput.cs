namespace Padoru.Diagnostics
{
    public interface IDebugOutput
    {
        void WriteToOuput(LogType logType, object message, string channel, object context);
    }
}
