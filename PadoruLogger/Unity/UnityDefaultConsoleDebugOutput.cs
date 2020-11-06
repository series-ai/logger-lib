using System.Collections;
using System.Collections.Generic;

namespace Padoru.Diagnostics
{
	public class UnityDefaultConsoleDebugOutput : IDebugOutput
	{
		public void WriteToOuput(LogType logType, object message, string channel, object context)
		{
			var unityContext = context as UnityEngine.Object;

			if (unityContext == null)
			{
				if (logType == LogType.Info)
				{
					UnityEngine.Debug.Log(message);
				}
				else if (logType == LogType.Warning)
				{
					UnityEngine.Debug.LogWarning(message);
				}
				else if (logType == LogType.Error || logType == LogType.Exception)
				{
					UnityEngine.Debug.LogError(message);
				}
			}
			else
			{
				if (logType == LogType.Info)
				{
					UnityEngine.Debug.Log(message, unityContext);
				}
				else if (logType == LogType.Warning)
				{
					UnityEngine.Debug.LogWarning(message, unityContext);
				}
				else if (logType == LogType.Error || logType == LogType.Exception)
				{
					UnityEngine.Debug.LogError(message, unityContext);
				}
			}
		}
	}
}
