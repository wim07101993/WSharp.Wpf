using System.Collections.ObjectModel;
using System.Diagnostics;
using WSharp.Logging;
using WSharp.Logging.Loggers;

namespace WSharp.Wpf.NetFramework.Demo.ViewModels
{
    public class LoggingViewModel
    {
        private readonly IMemoryLogger _memoryLogger;

        public LoggingViewModel(IMemoryLogger memoryLogger)
        {
            _memoryLogger = memoryLogger;
            _memoryLogger.Log(nameof(LoggingViewModel), "Hello world, this is a test");
            _memoryLogger.Log(
                source: nameof(LoggingViewModel),
                payload: new[] { "Hello world", "this is a test" },
                eventType: TraceEventType.Information,
                title: "test");
        }

        public ReadOnlyObservableCollection<ILogEntry> Logs => _memoryLogger.Logs;
    }
}
