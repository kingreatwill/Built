using System;
using System.Threading.Tasks;

namespace FM.ConsulInterop
{
    public static class InnerLogger
    {
        /// <summary>
        /// Occurs when [consul log].
        /// </summary>
        public static event Action<LoggerInfoEventArgs> ConsulLog;

        /// <summary>
        /// write log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        public static void Log(LoggerLevel level, string log)
        {
            ConsulLog?.Invoke(new LoggerInfoEventArgs
            {
                Level = level,
                Content = log
            });
        }

        /// <summary>
        /// write log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        public static async Task LogAsync(LoggerLevel level, string log)
        {
            await Task.Run(
                () =>
                ConsulLog?.Invoke(new LoggerInfoEventArgs
                {
                    Level = level,
                    Content = log
                })
            );
        }
    }

    public enum LoggerLevel
    {
        /// <summary>
        /// debug
        /// </summary>
        Debug,

        /// <summary>
        /// info
        /// </summary>
        Info,

        /// <summary>
        /// error
        /// </summary>
        Error
    }

    public class LoggerInfoEventArgs
    {
        public LoggerLevel Level { get; set; }
        public string Content { get; set; }
    }
}