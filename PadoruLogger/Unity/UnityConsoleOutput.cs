namespace Padoru.Diagnostics
{
	public class UnityConsoleOutput : IDebugOutput
	{
		public void WriteToOuput(LogType logType, object message, string channel, object context)
		{
			var unityContext = context as UnityEngine.Object;
			var finalMessage = message ?? string.Empty;

			if (unityContext == null)
			{
				if (logType == LogType.Info)
				{
					UnityEngine.Debug.Log(finalMessage);
				}
				else if (logType == LogType.Warning)
				{
					UnityEngine.Debug.LogWarning(finalMessage);
				}
				else if (logType == LogType.Error || logType == LogType.Exception)
				{
					UnityEngine.Debug.LogError(finalMessage);
				}
			}
			else
			{
				if (logType == LogType.Info)
				{
					UnityEngine.Debug.Log(finalMessage, unityContext);
				}
				else if (logType == LogType.Warning)
				{
					UnityEngine.Debug.LogWarning(finalMessage, unityContext);
				}
				else if (logType == LogType.Error || logType == LogType.Exception)
				{
					UnityEngine.Debug.LogError(finalMessage, unityContext);
				}
			}
		}
	}
}
