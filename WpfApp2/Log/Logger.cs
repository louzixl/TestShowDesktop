using System;

namespace WpfApp2.Log
{
    public static class Logger
    {
        public static Action<string> WriteMessage;

        public static Severity LogLevel { get; set; } = Severity.Info;

        public static void LogMessage(Severity level, string msg)
        {
            if (level < LogLevel)
                return;

            var outputMsg = $"{DateTime.Now} {level} {msg}";
            WriteMessage?.Invoke(outputMsg);
        }
    }
}
