using System;

namespace Senrihin_no_Kakuhoki
{
    public class LogManager
    {
        public LogManager(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public void DisplayMessage(LogLevel level, string message)
        {
            if (level < _logLevel)
                return;
            switch (level)
            {
                case LogLevel.Debug: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }

        LogLevel _logLevel;

        public enum LogLevel
        {
            Debug,
            Info,
            Error
        }
    }
}
