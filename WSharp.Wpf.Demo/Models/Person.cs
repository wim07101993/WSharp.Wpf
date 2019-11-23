using Prism.Mvvm;
using System;

namespace WSharp.Wpf.Demo.Models
{
    public class Person : BindableBase
    {
        private string _firstName;
        private string _lastName;
        //private DateTime _birthday;
        private Animal _pet;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        //public DateTime Birthday
        //{
        //    get => _birthday;
        //    set => SetProperty(ref _birthday, value);
        //}

        public Animal Pet
        {
            get => _pet;
            set => SetProperty(ref _pet, value);
        }
    }
}