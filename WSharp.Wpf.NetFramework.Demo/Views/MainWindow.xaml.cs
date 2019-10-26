using WSharp.Wpf.NetFramework.Demo.ViewModels;

namespace WSharp.Wpf.NetFramework.Demo.Views
{
    public partial class MainWindow
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public MainViewModel ViewModel
        {
            get => DataContext as MainViewModel;
            set => DataContext = value;
        }
    }
}
