using WSharp.Wpf.Demo.ViewModels;

namespace WSharp.Wpf.Demo.Views
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
