using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Services
{
    internal class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
