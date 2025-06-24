using CarRentalSystem.Interfaces;
using System.IO;

namespace CarRentalSystem.Utils
{
    public class Logger : ILogger
    {
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
        private readonly string _logFilePath = "CarRentalLog.txt";
        private readonly object _lock = new object();

        private Logger() { }

        public static Logger Instance => _instance.Value;

        public void LogInfo(string message) => Log("INFO", message);
        public void LogWarning(string message) => Log("WARNING", message);
        public void LogError(string message, Exception ex = null)
        {
            string fullMessage = ex != null ? $"{message} | Exception: {ex.Message}\nStackTrace: {ex.StackTrace}" : message;
            Log("ERROR", fullMessage);
        }

        private void Log(string level, string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now:G} [{level}] - {message}{Environment.NewLine}");
            }
        }
    }
}