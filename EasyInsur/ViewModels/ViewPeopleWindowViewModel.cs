using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using EasyInsur.Models;
using EasyInsur.Modules;

namespace EasyInsur.ViewModels
{
    public class ViewPeopleWindowViewModel : BindableBase
    {
        public ViewPeopleWindowViewModel()
        {
            AllPeople = DBMethods.GetPeople();
            PersonType = PersonType.Any;
            ReloadCommand = new DelegateCommand(() =>
            {
                AllPeople = DBMethods.GetPeople();
                PersonType = _personType;
            });
        }

        #region Properties

        private PersonType _personType;
        public PersonType PersonType
        {
            get => _personType;
            set
            {
                SetProperty(ref _personType, value);
                People = PersonType switch
                {
                    PersonType.Agent => Agents,
                    PersonType.Customer => Customers,
                    _ => AllPeople
                };
            }
        }

        private IEnumerable<Person> _allPeople;
        private IEnumerable<Person> AllPeople
        {
            get => _allPeople;
            set
            {
                SetProperty(ref _allPeople, value);
                var enumerable = _allPeople.ToList();
                Agents = enumerable.Where(p => p.Type == "Agent");
                Customers = enumerable.Where(p => p.Type == "Customer");
            } 
        }
        private IEnumerable<Person> Agents { get; set; }
        private IEnumerable<Person> Customers { get; set; }
        private IEnumerable<Person> _people;
        public IEnumerable<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }
        #endregion

        #region Delegates

        public DelegateCommand<string> SearchCommand { get; }
        public DelegateCommand<string> SaveCommand { get; }
        public DelegateCommand ReloadCommand { get; }


        #endregion
    }
}
