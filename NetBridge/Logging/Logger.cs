using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Logging
{
    /// <summary>
    /// Log levels. None is used to disable logging.
    /// </summary>
    public enum LogLevel
    {
        None = 0, // Used to disable logging
        Debug = 1, // Used for debugging
        Info = 2, // Used for informational messages
        Warning = 3, // Used for warnings
        Error = 4, // Used for errors
        Fatal = 5 // Used for fatal errors
    }

    public struct LogConfig
    {
        public LogLevel ConsoleLogLevel { get; set; }
        public LogLevel FileLogLevel { get; set; }
        public string LogFilePath { get; set; }
    }

    public class Logger
    {
        public LogConfig LogConfig { get; private set; }
        private readonly StreamWriter _writer;

        public Logger(LogConfig logConfig)
        {
            LogConfig = logConfig;

            // Only create a file writer if the file log level is greater than 0 and the log file path is not null.
            if(LogConfig.FileLogLevel > 0 && (LogConfig.LogFilePath != null))
            {
                _writer = new StreamWriter(LogConfig.LogFilePath, false);
            }
            else
            {
                // If there won't be any file logging, set the writer to null.
                _writer = StreamWriter.Null;
            }
        }

        public void Log(string message, LogLevel logLevel)
        {
            // Define the log color and check for invalid log levels.
            switch (logLevel)
            {
                case LogLevel.None:
                    return; // If the log level is none, return.
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta; // Debug messages are magenta.
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.DarkCyan; // Info messages are dark cyan.
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow; // Warning messages are yellow.
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red; // Error messages are red.
                    break;
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed; // Fatal messages are dark red.
                    break;
                default: // If the log level is not valid, log it and throw an exception.
                    this.Log("Provided a log level of 0 or less.", LogLevel.Error);     
                    throw new ArgumentException("Provided a log level of 0 or less.");
            }

            // Define the log message.
            string logMessage = string.Format("[{0}] {1}", logLevel.ToString(), message);

            // Write the log to the console.
            if (logLevel >= LogConfig.ConsoleLogLevel)
            {
                Console.WriteLine(logMessage);
                Console.ResetColor();
            }

            // Write the log to the file.
            if (logLevel >= LogConfig.FileLogLevel)
            {
                _writer.WriteLine(logMessage);
                _writer.Flush();
            }
        }

        public void Info(string message)
        {
            this.Log(message, LogLevel.Info);
        }

        public void Debug(string message)
        {
            this.Log(message, LogLevel.Debug);
        }

        public void Warning(string message)
        {
            this.Log(message, LogLevel.Warning);
        }

        public void Error(string message)
        {
            this.Log(message, LogLevel.Error);
        }

        public void Fatal(string message)
        {
            this.Log(message, LogLevel.Fatal);
        }
    }
}
