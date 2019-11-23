using Prism.Mvvm;
using System;
using System.Collections.Generic;
using WSharp.Wpf.Demo.Models;

namespace WSharp.Wpf.Demo.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(LoggingViewModel loggingViewModel)
        {
            LoggingViewModel = loggingViewModel;
        }

        public LoggingViewModel LoggingViewModel { get; }

        public List<Person> People { get; } = new List<Person>
        {
            new Person
            {
                FirstName = "Clark",
                LastName = "Kent",
                //Birthday = new DateTime(1966, 10, 5),
                Pet = new Animal
                {
                    Name = "Tiger",
                    Species = EAnimalSpecies.Dog
                }
            },
            new Person
            {
                FirstName = "Peter",
                LastName = "Parker",
                //Birthday = new DateTime(1962, 8, 15),
                Pet = new Animal
                {
                    Name = "Cuddles",
                    Species = EAnimalSpecies.Cat
                }
            }
        };
    }
}
