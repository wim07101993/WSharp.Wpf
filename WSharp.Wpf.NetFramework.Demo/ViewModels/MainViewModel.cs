using Prism.Mvvm;

namespace WSharp.Wpf.NetFramework.Demo.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(LoggingViewModel loggingViewModel)
        {
            LoggingViewModel = loggingViewModel;
        }

        public LoggingViewModel LoggingViewModel { get; }
    }
}
