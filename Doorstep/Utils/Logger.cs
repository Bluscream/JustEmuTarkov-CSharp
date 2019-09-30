using System;
using System.Diagnostics;
using System.IO;
// using NLog;
// using UnityEngine;

namespace Doorstop {
    public static class Logger {
        private const string LogDir = "Logs/";
        private static string LogFile = LogDir + "Doorstop.log";
        // private static readonly NLog.Logger NLogger = LogManager.GetCurrentClassLogger();
        // private static StreamWriter LogWriter;
        private static bool Initialized = false;
        private enum LogLevel { Trace, Debug, Info, Warn, Error, Fatal }

        private static void Init(bool clearLogs = true) {
            if (clearLogs) ClearLogs();
            // LogWriter = File.AppendText(LogFile);
            /*var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = LogFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logconsole);
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
            LogManager.Configuration = config;*/
        }

        private static bool ClearLogs()
        {
            Console.WriteLine("[Doorstop] Clearing Logs...");
            var failed = false;
            try {
                foreach (var file in Directory.GetFiles(LogDir)) {
                    try { File.Delete(file);
                    } catch (Exception) { Logger.Warn("Failed to delete {0}", file); failed = true; }
                }
                foreach (var folder in  Directory.GetDirectories(LogDir)) {
                    try { Directory.Delete(folder, true);
                    } catch (Exception) { Logger.Warn("Failed to delete {0}", folder); failed = true; }
                }
            } catch (Exception) { return false; }
            return failed;
        }

        public static void Trace(string msg, params object[] format) => log(LogLevel.Trace, msg: msg, format: format);
        public static void Debug(string msg, params object[] format) => log(LogLevel.Debug, msg: msg, format: format);
        public static void Log(string msg, params object[] format) => log(LogLevel.Info, msg: msg, format: format);
        public static void Info(string msg, params object[] format) => log(LogLevel.Info, msg: msg, format: format);
        public static void Warn(string msg, params object[] format) => log(LogLevel.Warn, msg: msg, format: format);
        public static void Error(string msg, params object[] format) => log(LogLevel.Error, msg: msg, format: format);
        public static void Fatal(string msg, params object[] format) => log(LogLevel.Fatal, msg: msg, format: format);
        private static void log(LogLevel logLevel, string msg, params object[] format) {
            if (!Initialized) { Initialized = true; Init(); }
            var formatted = string.Format(msg, format);
            // NLogger.Info(formatted); UnityEngine.Debug.Log(formatted);
             string timestamp = DateTime.Now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            StackFrame frame = new StackFrame(2); var method = frame.GetMethod(); var cName = method.DeclaringType.Name; var mName = method.Name;
            var line = $"[{timestamp}] <Doorstep> {logLevel} - {cName}.{mName}: {formatted}";
            Console.WriteLine(line);
            // LogWriter.WriteLine(line);
            using (StreamWriter w = File.AppendText(LogFile)) { w.WriteLine(line); }
        }
    }
}