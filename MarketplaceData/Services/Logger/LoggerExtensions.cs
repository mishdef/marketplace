using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Services.Logger
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddCustomConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();
            return builder;
        }
    }
}
