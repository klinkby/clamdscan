using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Klinkby.ClamTests
{
    internal class LoggerAdapter : ILogger
    {
        private readonly ITestOutputHelper logger;
        private int indent;

        public LoggerAdapter(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            logger.WriteLine(new string(' ', indent++) + state?.ToString());
            return new Scope(() => --indent);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            logger.WriteLine(new string(' ', indent) + $"{logLevel}\t{eventId}\t{formatter(state, exception)}");
        }

        class Scope : IDisposable
        {
            private readonly Action decrementer;

            internal Scope(Action decrementer)
            {
                this.decrementer = decrementer;
            }


            public void Dispose()
            {
                decrementer();
            }
        }
    }
}
