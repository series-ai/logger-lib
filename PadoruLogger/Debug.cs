using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Padoru.Diagnostics
{
    public static class Debug
    {
        private const string DEFAULT_CHANNEL_NAME = "Default";

        private static StackTrace currentStackTrace;
        private static StackFrame[] stackFrames;
        private static int frameCount;

        private static StackTraceFormatter stackTraceFormatter = null;
        private static LogFormatter logFormatter = null;
        private static LogSettings settings = null;
        private static bool isConfigured = false;

        public static event Action<string> onLog;
        public static event Action<string> onLogWarning;
        public static event Action<string> onLogError;

        // Used for stackframe caching
        private static int currentFrame = 0;
        private static int cachedFrame = -1;

        private static string ClassName
        {
            get
            {
                return GetStackFrame().GetMethod().DeclaringType.Name;
            }
        }
        private static string MethodName
        {
            get
            {
                return GetStackFrame().GetMethod().Name;
            }
        }
               
        #region Public Interface
        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings)
        {
            Configure(settings, new DefaultLogFormatter(), new DefaultStackTraceFormatter());
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, LogFormatter logFormatter)
        {
            Configure(settings, logFormatter, new DefaultStackTraceFormatter());
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, StackTraceFormatter stackTraceFormatter)
        {
            Configure(settings, new DefaultLogFormatter(), stackTraceFormatter);
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, LogFormatter logFormatter, StackTraceFormatter stackTraceFormatter)
        {
            try
            {
                Debug.settings = settings;
                if (settings == null)
                {
                    throw new Exception("Settings cannot be null");
                }

                Debug.stackTraceFormatter = stackTraceFormatter;
                if (stackTraceFormatter == null)
                {
                    throw new Exception("StackTrace formatter cannot be null");
                }

                Debug.logFormatter = logFormatter;
                if (logFormatter == null)
                {
                    throw new Exception("Log formatter formatter cannot be null");
                }

                isConfigured = true;
            }
            catch (Exception e)
            {
                isConfigured = false;
                throw new Exception($"Logger could not be configured: {e.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="indent">Kept for backward compatibility</param>
        public static void Log(object message = null, string channel = DEFAULT_CHANNEL_NAME)
        {
            InternalLog(LogType.Info, message, channel, onLog);

        }
        public static void LogWarning(object message = null, string channel = DEFAULT_CHANNEL_NAME)
        {
            InternalLog(LogType.Warning, message, channel, onLogWarning);
        }

        public static void LogError(object message = null, string channel = DEFAULT_CHANNEL_NAME)
        {
            InternalLog(LogType.Error, message, channel, onLogError);
        }
        #endregion Public Interface

        #region Private Methods

        private static void InternalLog(LogType logType, object message, string channel, Action<string> callback)
        {
            if (!isConfigured)
            {
                throw new Exception("Trying to use PadoruEngine.Diagnostics.Debug, but it failed. Call Configure() before using this logger.");
            }

            if (logType < settings.LogType) return;

            bool printStacktrace = (logType >= settings.StacktraceLogType);

            var logData = GetLogData(message, logType, printStacktrace, channel);

            var formattedLog = GetFromattedLogMessage(logData);

            callback?.Invoke(formattedLog);

            currentFrame++;
        }

        private static LogData GetLogData(object message, LogType logType, bool printStacktrace, string channel)
        {
            CacheStackTrace();

            var logData = new LogData()
            {
                message = message,
                logType = logType,
                dateTime = DateTime.Now,
                channel = channel,
                contextClass = ClassName,
                contextMethod = MethodName,
                stackTrace = printStacktrace ? currentStackTrace : null,
            };

            return logData;
        }

        private static string GetFromattedLogMessage(LogData logData)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(logFormatter.GetFormattedLog(logData));
            sb.Append(Environment.NewLine);

            if (logData.stackTrace != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(stackTraceFormatter.GetFormattedStackTrace(logData.stackTrace));
            }

            return sb.ToString();
        }

        #region StackTrace
        private static StackFrame GetStackFrame()
        {
            // Find the position of Debug.Log in the callstack 
            // and point to the next immediate element on the stack, which is the one we are interested in

            int debugCallPos = 0;
            foreach (var f in stackFrames)
            {
                var clss = f.GetMethod().DeclaringType;
                var methodName = f.GetMethod().Name;
                if (clss == typeof(Debug) && methodName.StartsWith("Log"))
                {
                    break;
                }
                debugCallPos++;
            }
            return stackFrames[debugCallPos + 1];
        }

        private static void CacheStackTrace()
        {
            if (currentFrame != cachedFrame)
            {
                // Invalidate cache
                currentStackTrace = new StackTrace(true);
                stackFrames = currentStackTrace.GetFrames();
                frameCount = currentStackTrace.FrameCount;
                cachedFrame = currentFrame;
            }
        }
        #endregion Stacktrace

        #region File Writing
        private static void WriteToStorage(object message, string path)
        {
            if (isConfigured)
            {
                try
                {
                    /*
                    if (sw == null)
                    {
                        sw = new StreamWriter(path: Settings.LogAbsolutePath, append: true);
                        bool saved = Settings.LogToExternalLogger;
                        Settings.LogToExternalLogger = false;
                        Log("--------------Log file opened--------------");
                        Settings.LogToExternalLogger = saved;
                    }
                    sw.WriteLine(message);
                    */
                    //File.AppendAllText(Settings.LogAbsolutePath, $"{message.ToString()}{'\n'}");
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not write to log file: {e.Message}");
                }
            }
            else
            {
                throw new Exception("Logger must be configured before trying to log. Call Debug.Configure method!");
            }
        }
        #endregion File Writing

        #endregion Private Methods
    }
}
