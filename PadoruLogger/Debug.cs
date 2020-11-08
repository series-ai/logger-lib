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
        private static List<IDebugOutput> outputs;
        private static bool isConfigured = false;

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

                outputs = new List<IDebugOutput>();

                isConfigured = true;
            }
            catch (Exception e)
            {
                isConfigured = false;
                throw new Exception($"Logger could not be configured: {e.Message}");
            }
        }

        public static void AddOutput(IDebugOutput output)
        {
            if(output == null)
            {
                throw new Exception("The given debug output is null");
            }

            outputs.Add(output);
        }

        public static void Log(object message = null)
        {
            InternalLog(LogType.Info, message, DEFAULT_CHANNEL_NAME, null);
        }

        public static void Log(object message, string channel)
        {
            InternalLog(LogType.Info, message, channel, null);
        }

        public static void Log(object message, object context)
        {
            InternalLog(LogType.Info, message, DEFAULT_CHANNEL_NAME, context);
        }

        public static void Log(object message, string channel, object context)
        {
            InternalLog(LogType.Info, message, channel, context);
        }


        public static void LogWarning(object message = null)
        {
            InternalLog(LogType.Warning, message, DEFAULT_CHANNEL_NAME, null);
        }

        public static void LogWarning(object message, string channel)
        {
            InternalLog(LogType.Warning, message, channel, null);
        }

        public static void LogWarning(object message, object context)
        {
            InternalLog(LogType.Warning, message, DEFAULT_CHANNEL_NAME, context);
        }

        public static void LogWarning(object message, string channel, object context)
        {
            InternalLog(LogType.Warning, message, channel, context);
        }

        public static void LogError(object message = null)
        {
            InternalLog(LogType.Error, message, DEFAULT_CHANNEL_NAME, null);
        }

        public static void LogError(object message, string channel)
        {
            InternalLog(LogType.Error, message, channel, null);
        }

        public static void LogError(object message, object context)
        {
            InternalLog(LogType.Error, message, DEFAULT_CHANNEL_NAME, context);
        }

        public static void LogError(object message, string channel, object context)
        {
            InternalLog(LogType.Error, message, channel, context);
        }

        public static void LogException(Exception e)
        {
            InternalLog(LogType.Exception, e.Message, DEFAULT_CHANNEL_NAME, null, e.StackTrace);
        }

        public static void LogException(Exception e, string channel)
        {
            InternalLog(LogType.Exception, e.Message, channel, null, e.StackTrace);
        }

        public static void LogException(Exception e, object context)
        {
            InternalLog(LogType.Exception, e.Message, DEFAULT_CHANNEL_NAME, context, e.StackTrace);
        }

        public static void LogException(Exception e, string channel, object context)
        {
            InternalLog(LogType.Exception, e.Message, channel, context, e.StackTrace);
        }
        #endregion Public Interface

        #region Private Methods

        private static void InternalLog(LogType logType, object message, string channel, object context)
        {
            if (!isConfigured)
            {
                AutoCongifure();
            }

            if (logType < settings.LogType) return;

            bool printStacktrace = (logType >= settings.StacktraceLogType);

            var logData = GetLogData(message, logType, printStacktrace, channel, context);

            var formattedLog = GetFromattedLogMessage(logData);

            // Call outputs
            foreach (var output in outputs)
            {
                output.WriteToOuput(logType, formattedLog, channel, context);
            }

            currentFrame++;
        }

        private static void InternalLog(LogType logType, object message, string channel, object context, string stackTrace)
        {
            if (!isConfigured)
            {
                AutoCongifure();
            }

            if (logType < settings.LogType) return;

            bool printStacktrace = (logType >= settings.StacktraceLogType);

            var logData = GetLogData(message, logType, printStacktrace, channel, context);

            var formattedLog = GetFromattedLogMessage(logData, stackTrace);

            // Call outputs
            foreach (var output in outputs)
            {
                output.WriteToOuput(logType, formattedLog, channel, context);
            }

            currentFrame++;
        }

        private static LogData GetLogData(object message, LogType logType, bool printStacktrace, string channel, object context)
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
                context = context,
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
                sb.Append("[Stacktrace]");
                sb.Append(Environment.NewLine);
                sb.Append(stackTraceFormatter.GetFormattedStackTrace(logData.stackTrace));
            }

            return sb.ToString();
        }

        private static string GetFromattedLogMessage(LogData logData, string customStackTrace)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(logFormatter.GetFormattedLog(logData));
            sb.Append(Environment.NewLine);

            if (logData.stackTrace != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append("[Stacktrace]");
                sb.Append(Environment.NewLine);
                sb.Append(customStackTrace);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private static void AutoCongifure()
        {
            var defaultLogSettings = new LogSettings()
            {
                LogType = LogType.Info,
                StacktraceLogType = LogType.Info,
            };

            var defaultLogFormatter = new UnityDefaultLogFormatter();
            var defaultStackTraceFormatter = new UnityDefaultStackTraceFormatter();

            var defaultOutput = new UnityConsoleOutput();

            Configure(defaultLogSettings, defaultLogFormatter, defaultStackTraceFormatter);

            AddOutput(defaultOutput);

            Debug.LogWarning($"Tried to use Padoru.Diagnostics.Debug without configuring it first. Logger auto-configured itself with default options.");
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
        
        #endregion Private Methods
    }
}
