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
            ReloadCommand = new DelegateCommand(() =>
            {
                _allPersons = DBMethods.GetPeople();
                PersonType = _personType;
            });
        }

        #region Properties

        private PersonType _personType = PersonType.Agent;
        public PersonType PersonType
        {
            get => _personType;
            set
            {
                SetProperty(ref _personType, value);
                People = PersonType switch
                {
                    PersonType.Agent => _allPersons.Where(p => p.Type == "Agent"),
                    PersonType.Customer => _allPersons.Where(p => p.Type == "Customer"),
                    _ => _allPersons
                };
            }
        }

        private IEnumerable<Person> _allPersons = DBMethods.GetPeople();
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
