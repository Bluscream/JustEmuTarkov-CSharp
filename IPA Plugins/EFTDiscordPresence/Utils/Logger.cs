using System;
using System.Diagnostics;
using System.IO;

// using NLog;
// using UnityEngine;

namespace EFTDiscordPresence
{
    public static class Logger
    {
        private const string LogFile = "Logs/Plugins.log";

        // private static readonly NLog.Logger NLogger = LogManager.GetCurrentClassLogger();
        private static StreamWriter LogWriter;

        private static bool Initialized = false;

        private enum LogLevel { Trace, Debug, Info, Warn, Error, Fatal }

        private static void Init()
        {
            Initialized = true;
            try
            {
                LogWriter = File.AppendText(LogFile);
                /*var config = new NLog.Config.LoggingConfiguration();
                var logfile = new NLog.Targets.FileTarget("logfile") { FileName = LogFile };
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
                LogManager.Configuration = config;*/
            }
            catch (Exception ex) { Error("Unable to initialize LogWriter: {0}", ex.Message); }
        }

        public static void Trace(string msg, params object[] format) => log(LogLevel.Trace, msg: msg, format: format);

        public static void Debug(string msg, params object[] format) => log(LogLevel.Debug, msg: msg, format: format);

        public static void Log(string msg, params object[] format) => log(LogLevel.Info, msg: msg, format: format);

        public static void Info(string msg, params object[] format) => log(LogLevel.Info, msg: msg, format: format);

        public static void Warn(string msg, params object[] format) => log(LogLevel.Warn, msg: msg, format: format);

        public static void Error(string msg, params object[] format) => log(LogLevel.Error, msg: msg, format: format);

        public static void Fatal(string msg, params object[] format) => log(LogLevel.Fatal, msg: msg, format: format);

        private static void log(LogLevel logLevel, string msg, params object[] format)
        {
            if (!Initialized) Init();
            var formatted = string.Format(msg, format);
            // NLogger.Info(formatted); UnityEngine.Debug.Log(formatted);
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            StackFrame frame = new StackFrame(2); var method = frame.GetMethod(); var cName = method.DeclaringType.Name; var mName = method.Name;
            var line = $"[{timestamp}] <EFTDiscordPresence> {logLevel} - {cName}.{mName}: {formatted}";
            Console.WriteLine(line);
            if (LogWriter != null) LogWriter.WriteLine(line);
        }
    }

    /*public class CustomLogger : ILogger
    {
        private readonly string CategoryName;
        private readonly string _logPrefix;

        public CustomLogger(string categoryName, string logPrefix)
        {
            CategoryName = categoryName;
            _logPrefix = logPrefix;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, object eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = _logPrefix;
            if (formatter != null)
            {
                message += formatter(state, exception);
            }
            // Implement log writter as you want. I am using Console
            Console.WriteLine($"{logLevel.ToString()} - {eventId} - {CategoryName} - {message}");
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }*/
}