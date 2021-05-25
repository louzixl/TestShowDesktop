using System;
using System.IO;

namespace WpfApp2.Log
{
    public class FileLogger
    {
        private readonly string logPath;

        public FileLogger(string path)
        {
            logPath = path;
            Logger.WriteMessage += LogMessage;
        }

        public void DetachLog() => Logger.WriteMessage -= LogMessage;

        private void LogMessage(string msg)
        {
            try
            {
                using (var log = File.AppendText(logPath))
                {
                    log.WriteLine(msg);
                    log.Flush();
                }
            }
            catch (Exception)
            { 
            }
        }
    }
}
