using Prism.Mvvm;

namespace WSharp.Wpf.Demo.Models
{
    public class Animal : BindableBase
    {
        private string _name;
        private EAnimalSpecies _species;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public EAnimalSpecies Species
        {
            get => _species;
            set => SetProperty(ref _species, value);
        }
    }
}
