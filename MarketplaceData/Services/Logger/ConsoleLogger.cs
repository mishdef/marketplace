using Microsoft.Extensions.Logging;
using System;

namespace MarketplaceData
{
    public class ConsoleLogger : ILogger
    {
        private readonly string _name;

        private List<Exception> _exceptions = new List<Exception>();

        public ConsoleLogger(string name)
        {
            _name = name;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.Trace;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string message = formatter(state, exception);

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetColorForLogLevel(logLevel);

            Console.Write($"[{logLevel} ({_exceptions.Count + 1})]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($": {message}\r\n");

            if (exception != null)
            {
                Console.WriteLine($"[{_name}] Exception: {exception.Message}");
                Console.ForegroundColor = originalColor;                
                Console.Beep();

                _exceptions.Add(exception);
            }

            Console.ForegroundColor = originalColor;
        }

        private ConsoleColor GetColorForLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Error or LogLevel.Critical => ConsoleColor.Red,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Information => ConsoleColor.Cyan,
            _ => ConsoleColor.Gray
        };
    }
}