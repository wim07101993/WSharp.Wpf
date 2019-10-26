using System.Threading;
using System.Windows;
using Unity;
using WSharp.Extensions;
using WSharp.Logging;
using WSharp.Logging.Loggers;
using WSharp.Wpf.NetFramework.Demo.ViewModels;
using WSharp.Wpf.NetFramework.Demo.Views;

namespace WSharp.Wpf.NetFramework.Demo
{
    public partial class App
    {
        public IUnityContainer UnityContainer { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            InitSettings();
            InitUnityContainer();
            InitLogging();

            base.OnStartup(e);

            MainWindow = UnityContainer.Resolve<MainWindow>();
            MainWindow.Show();
            UnityContainer.Resolve<ILogger>().Log("Application", "Show main window");
        }

        private void InitSettings()
        {
        }

        private void InitUnityContainer()
        {
            UnityContainer = new UnityContainer()
#if DEBUG
                .EnableDiagnostic()
#endif
                .RegisterWSharp()
                .RegisterSingleton<ILogger, ILogDispatcher>()
                .RegisterType<LoggingViewModel>()
                .RegisterType<MainViewModel>();
        }

        private void InitLogging()
        {
            var factory = UnityContainer.Resolve<ILogDispatcherFactory>();
            factory
                .RegisterLogDispatcherType<LogDispatcher>()
                .RegisterSingleton<IMemoryLogger, MemoryLogger>()
                .RegisterSingleton<IConsoleLogger, ConsoleLogger>();
        }
    }
}
