using Prism.Mvvm;

namespace WSharp.Wpf.Demo.ViewModels
{
    public class TextBoxesViewModel : BindableBase
    {
        private string _name;
        private string _phone;
        private string _comment;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }
    }
}
