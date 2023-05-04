using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

namespace Padoru.Diagnostics
{
    public static class Debug
    {
        private const string DEFAULT_CHANNEL_NAME = "Default";
        
        private static UnityConsoleOutput defaultUnityConsoleOutput;
        private static List<RuntimePlatform> unsupportedPlatforms;
        private static IStackTraceFormatter stackTraceFormatter;
        private static ILogFormatter logFormatter;
        private static LogSettings settings;
        private static List<IDebugOutput> outputs;
        private static bool isConfigured;
               
        #region Public Interface
        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings)
        {
            Configure(settings, new UnityDefaultLogFormatter(), new UnityDefaultStackTraceFormatter());
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, ILogFormatter logFormatter)
        {
            Configure(settings, logFormatter, new UnityDefaultStackTraceFormatter());
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, IStackTraceFormatter stackTraceFormatter)
        {
            Configure(settings, new UnityDefaultLogFormatter(), stackTraceFormatter);
        }

        /// <summary>
        /// Mandatory method to start using the logger
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="resetLogFileIfExists"></param>
        public static void Configure(LogSettings settings, ILogFormatter logFormatter, IStackTraceFormatter stackTraceFormatter)
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

                unsupportedPlatforms = settings.UnsupportedPlatforms;
                
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
            LogException(null, e, null);
        }

        public static void LogException(object messageHeader, Exception e)
        {
            LogException(messageHeader, e, null);
        }

        public static void LogException(Exception e, object context)
        {
            LogException(null, e, context);
        }
        
        public static void LogException(object messageHeader, Exception e, object context)
        {
            var message = messageHeader != null ? $"{messageHeader}. {e.Message}" : e.Message;
            
            InternalLog(LogType.Exception, message, DEFAULT_CHANNEL_NAME, context, e.StackTrace);
        }
        #endregion Public Interface

        #region Private Methods

        private static void InternalLog(LogType logType, object message, string channel, object context, string stackTrace = null)
        {
            message = message ?? string.Empty;
            
            if (!IsPlatformSupported())
            {
                FallbackLog(logType, message, channel, context);
                return;
            }

            if (!isConfigured)
            {
                AutoConfigure();
            }

            if (logType < settings.LogType) return;

            var printStacktrace = (logType >= settings.StacktraceLogType);

            var logData = GetLogData(message, logType, printStacktrace, channel, context);

            var formattedLog = GetFormattedLogMessage(logData, stackTrace);

            // Call outputs
            foreach (var output in outputs)
            {
                output.WriteToOuput(logType, formattedLog, channel, context);
            }

            StackTraceUtils.CurrentFrame++;
        }

        private static void FallbackLog(LogType logType, object message, string channel, object context)
        {
            if (defaultUnityConsoleOutput == null)
            {
                defaultUnityConsoleOutput = new UnityConsoleOutput();
            }

            var finalMessage = channel != null ? $"[{channel}] {message}" : message;

            defaultUnityConsoleOutput.WriteToOuput(logType, finalMessage, channel, context);
        }

        private static bool IsPlatformSupported()
        {
            if (unsupportedPlatforms == null)
            {
                return true;
            }
            
            foreach (var platform in unsupportedPlatforms)
            {
                if (platform == Application.platform)
                {
                    return false;
                }
            }

            return true;
        }

        private static LogData GetLogData(object message, LogType logType, bool printStacktrace, string channel, object context)
        {
            StackTraceUtils.CacheStackTrace();

            var logData = new LogData()
            {
                message = message,
                logType = logType,
                dateTime = DateTime.Now,
                channel = channel,
                contextClass = StackTraceUtils.ClassName,
                contextMethod = StackTraceUtils.MethodName,
                stackTrace = printStacktrace ? StackTraceUtils.CurrentStackTrace : null,
                context = context,
            };

            return logData;
        }

        private static string GetFormattedLogMessage(LogData logData, string customStackTrace)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(logFormatter.GetFormattedLog(logData));
            sb.Append(Environment.NewLine);

            if (logData.stackTrace != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append("[Stacktrace]");
                sb.Append(Environment.NewLine);

                if (customStackTrace != null)
                {
                    sb.Append(customStackTrace);
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    sb.Append(stackTraceFormatter.GetFormattedStackTrace(logData.stackTrace));
                }
            }

            return sb.ToString();
        }

        private static void AutoConfigure()
        {
            var defaultLogSettings = new LogSettings()
            {
                LogType = LogType.Info,
                StacktraceLogType = LogType.Info,
                UnsupportedPlatforms = new List<RuntimePlatform>()
                {
                    RuntimePlatform.IPhonePlayer,
                    RuntimePlatform.WebGLPlayer,
                },
            };

            var defaultLogFormatter = new UnityDefaultLogFormatter();
            var defaultStackTraceFormatter = new UnityDefaultStackTraceFormatter();

            var defaultOutput = new UnityConsoleOutput();

            Configure(defaultLogSettings, defaultLogFormatter, defaultStackTraceFormatter);

            AddOutput(defaultOutput);

            LogWarning($"Tried to use Padoru.Diagnostics.Debug without configuring it first. Logger auto-configured itself with default options.");
        }
        
        #endregion Private Methods
    }
}
